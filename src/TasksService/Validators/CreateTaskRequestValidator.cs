using FluentValidation;
using TasksService.Contracts;

namespace TasksService.Validators;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MinimumLength(3)
            .WithMessage("Title must have at least 3 characters.")
            .MaximumLength(150)
            .WithMessage("Title cannot exceed 150 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.DueDate)
            .NotNull()
            .WithMessage("DueDate is required.")
            .Must(BeAFutureDate)
            .WithMessage("DueDate must be in the future.");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Invalid priority value.");
    }

    private bool BeAFutureDate(DateTime? date)
    {
        if (!date.HasValue)
            return false;

        return date.Value > DateTime.UtcNow;
    }
}