using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Models;
using Domain.Interfaces;
using MediatR;

namespace Application.Tasks.Queries.GetTaskById;

internal sealed class GetTaskByIdHandler : IRequestHandler<GetTaskByIdQuery, TaskDto>
{
    private readonly ITaskRepository _repository;

    public GetTaskByIdHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.TaskItem), request.Id);

        return task.ToDto();
    }
}
