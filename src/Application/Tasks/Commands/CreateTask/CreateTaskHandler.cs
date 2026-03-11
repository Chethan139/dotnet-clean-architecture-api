using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Tasks.Commands.CreateTask;

internal sealed class CreateTaskHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly ITaskRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CreateTaskHandler(ITaskRepository repository, ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var createdBy = _currentUser.UserName ?? "system";

        var task = TaskItem.Create(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.AssignedTo,
            createdBy);

        await _repository.AddAsync(task, cancellationToken);

        return task.ToDto();
    }
}
