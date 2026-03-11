using System.Collections.Generic;
using Application.Common.Models;
using MediatR;

namespace Application.Tasks.Queries.GetAllTasks;

public sealed record GetAllTasksQuery(
    string? Status = null,
    string? AssignedTo = null
) : IRequest<IReadOnlyList<TaskDto>>;
