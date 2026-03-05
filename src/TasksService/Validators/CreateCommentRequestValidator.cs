using FluentValidation;
using TasksService.Contracts;

namespace TasksService.Validators;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required.")
            .MinimumLength(3)
            .WithMessage("Content must have at least 3 characters.")
            .MaximumLength(1000)
            .WithMessage("Content cannot exceed 1000 characters.")
            .Must(content => !string.IsNullOrWhiteSpace(content))
            .WithMessage("Content cannot be empty or whitespace.");
    }
}