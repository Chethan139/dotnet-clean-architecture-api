using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Tasks.Commands.DeleteTask;

internal sealed class DeleteTaskHandler : IRequestHandler<DeleteTaskCommand>
{
    private readonly ITaskRepository _repository;

    public DeleteTaskHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.TaskItem), request.Id);

        task.SoftDelete();

        await _repository.DeleteAsync(task, cancellationToken);
    }
}
