using TasksService.Domain.Enums;

namespace TasksService.Contracts;

public sealed class TaskResponse
{
    public Guid Id { get; init; }

    public string Title { get; init; } = default!;

    public string? Description { get; init; }

    public DateTime? DueDate { get; init; }

    public TaskPriority Priority { get; init; }

    public Domain.Enums.TaskStatus Status { get; init; }

    public Guid CreatedByUserId { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }
}