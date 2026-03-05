using FluentValidation;
using TasksService.Contracts;

namespace TasksService.Validators;

public class TaskQueryParametersValidator : AbstractValidator<TaskQueryParameters>
{
    private static readonly string[] AllowedSortFields =
    {
        "createdat",
        "duedate",
        "priority"
    };

    public TaskQueryParametersValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1.");

        RuleFor(x => x.Size)
            .InclusiveBetween(1, 100)
            .WithMessage("Size must be between 1 and 100.");

        RuleFor(x => x.SortBy)
            .Must(BeValidSortField)
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy))
            .WithMessage($"SortBy must be one of: {string.Join(", ", AllowedSortFields)}");

        RuleFor(x => x)
            .Must(HaveValidDateRange)
            .WithMessage("DueDateFrom must be earlier than or equal to DueDateTo.");
    }

    private bool BeValidSortField(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return true;

        return AllowedSortFields.Contains(sortBy.ToLower());
    }

    private bool HaveValidDateRange(TaskQueryParameters parameters)
    {
        if (parameters.DueDateFrom.HasValue && parameters.DueDateTo.HasValue)
        {
            return parameters.DueDateFrom <= parameters.DueDateTo;
        }

        return true;
    }
}