using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Models;
using Domain.Interfaces;
using MediatR;

namespace Application.Tasks.Commands.UpdateTask;

internal sealed class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
{
    private readonly ITaskRepository _repository;

    public UpdateTaskHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.TaskItem), request.Id);

        task.UpdateDetails(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.AssignedTo);

        task.UpdateStatus(request.Status);

        await _repository.UpdateAsync(task, cancellationToken);

        return task.ToDto();
    }
}
