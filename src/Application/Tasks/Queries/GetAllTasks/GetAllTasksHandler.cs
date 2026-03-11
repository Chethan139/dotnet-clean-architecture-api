using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Tasks.Queries.GetAllTasks;

internal sealed class GetAllTasksHandler : IRequestHandler<GetAllTasksQuery, IReadOnlyList<TaskDto>>
{
    private readonly ITaskRepository _repository;

    public GetAllTasksHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TaskDto>> Handle(
        GetAllTasksQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<TaskItem> tasks;

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<TaskStatus>(request.Status, ignoreCase: true, out var parsedStatus))
        {
            tasks = await _repository.GetByStatusAsync(parsedStatus, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(request.AssignedTo))
        {
            tasks = await _repository.GetByAssigneeAsync(request.AssignedTo, cancellationToken);
        }
        else
        {
            tasks = await _repository.GetAllAsync(cancellationToken);
        }

        return tasks.Select(t => t.ToDto()).ToList().AsReadOnly();
    }
}
