using System;
using Application.Common.Models;
using Domain.Entities;
using MediatR;

namespace Application.Tasks.Commands.UpdateTask;

public sealed record UpdateTaskCommand(
    Guid Id,
    string Title,
    string Description,
    TaskPriority Priority,
    TaskStatus Status,
    DateTime DueDate,
    string AssignedTo
) : IRequest<TaskDto>;
