using System;
using Application.Common.Models;
using Domain.Entities;
using MediatR;

namespace Application.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    string Title,
    string Description,
    TaskPriority Priority,
    DateTime DueDate,
    string AssignedTo
) : IRequest<TaskDto>;
