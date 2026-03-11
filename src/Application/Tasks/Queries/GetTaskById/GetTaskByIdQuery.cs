using System;
using Application.Common.Models;
using MediatR;

namespace Application.Tasks.Queries.GetTaskById;

public sealed record GetTaskByIdQuery(Guid Id) : IRequest<TaskDto>;
