using System.ComponentModel.DataAnnotations;
using TasksService.Domain.Enums;

namespace TasksService.Contracts;

public sealed class UpdateTaskRequest
{
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Title { get; init; } = default!;

    [StringLength(2000)]
    public string? Description { get; init; }

    [Required]
    public DateTime DueDate { get; init; }

    [Required]
    public TaskPriority Priority { get; init; }

    [Required]
    public Domain.Enums.TaskStatus Status { get; init; }
}