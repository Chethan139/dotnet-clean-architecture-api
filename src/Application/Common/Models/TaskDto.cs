using System;
using Domain.Entities;

namespace Application.Common.Models;

public sealed record TaskDto(
    Guid Id,
    string Title,
    string Description,
    string Status,
    string Priority,
    DateTime DueDate,
    string AssignedTo,
    string CreatedBy,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public static class TaskDtoExtensions
{
    public static TaskDto ToDto(this TaskItem task) => new(
        task.Id,
        task.Title,
        task.Description,
        task.Status.ToString(),
        task.Priority.ToString(),
        task.DueDate,
        task.AssignedTo,
        task.CreatedBy,
        task.CreatedAt,
        task.UpdatedAt
    );
}
