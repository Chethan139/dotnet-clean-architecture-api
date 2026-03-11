using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for request {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var (statusCode, title, errors) = exception switch
        {
            NotFoundException notFound => (
                HttpStatusCode.NotFound,
                "Resource Not Found",
                (object?)null),

            Application.Common.Exceptions.ValidationException validation => (
                HttpStatusCode.UnprocessableEntity,
                "Validation Failed",
                (object?)validation.Errors),

            ArgumentException argument => (
                HttpStatusCode.BadRequest,
                "Bad Request",
                (object?)null),

            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                "Unauthorized",
                (object?)null),

            _ => (
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.",
                (object?)null)
        };

        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new
        {
            type = $"https://httpstatuses.com/{(int)statusCode}",
            title,
            status = (int)statusCode,
            detail = exception.Message,
            errors,
            traceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(problemDetails, JsonOptions);
        await context.Response.WriteAsync(json);
    }
}
