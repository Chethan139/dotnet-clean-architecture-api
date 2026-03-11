using System;
using FluentValidation;

namespace Application.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Priority must be a valid value (Low, Medium, High, Critical).");

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Due date cannot be in the past.");

        RuleFor(x => x.AssignedTo)
            .MaximumLength(100).WithMessage("AssignedTo must not exceed 100 characters.");
    }
}
