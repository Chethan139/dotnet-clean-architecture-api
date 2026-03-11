using System;
using MediatR;

namespace Application.Tasks.Commands.DeleteTask;

public sealed record DeleteTaskCommand(Guid Id) : IRequest;
