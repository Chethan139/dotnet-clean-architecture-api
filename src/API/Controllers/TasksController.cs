using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Application.Tasks.Commands.CreateTask;
using Application.Tasks.Commands.DeleteTask;
using Application.Tasks.Commands.UpdateTask;
using Application.Tasks.Queries.GetAllTasks;
using Application.Tasks.Queries.GetTaskById;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public sealed class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all tasks, with optional filtering by status or assignee.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all tasks", Tags = new[] { "Tasks" })]
    [ProducesResponseType(typeof(IReadOnlyList<TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetAll(
        [FromQuery] string? status = null,
        [FromQuery] string? assignedTo = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAllTasksQuery(status, assignedTo), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific task by its unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get task by ID", Tags = new[] { "Tasks" })]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TaskDto>> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetTaskByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new task", Tags = new[] { "Tasks" })]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TaskDto>> Create(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateTaskCommand(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.AssignedTo);

        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update an existing task", Tags = new[] { "Tasks" })]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TaskDto>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateTaskCommand(
            id,
            request.Title,
            request.Description,
            request.Priority,
            request.Status,
            request.DueDate,
            request.AssignedTo);

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Soft-deletes a task by ID.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete a task", Tags = new[] { "Tasks" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteTaskCommand(id), cancellationToken);
        return NoContent();
    }
}

// ----------- Request models (kept in controller file for locality) -----------

public sealed record CreateTaskRequest(
    string Title,
    string Description,
    TaskPriority Priority,
    DateTime DueDate,
    string AssignedTo
);

public sealed record UpdateTaskRequest(
    string Title,
    string Description,
    TaskPriority Priority,
    TaskStatus Status,
    DateTime DueDate,
    string AssignedTo
);
