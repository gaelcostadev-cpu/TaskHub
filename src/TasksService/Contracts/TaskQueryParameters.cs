using TasksService.Domain.Enums;

namespace TasksService.Contracts;

public class TaskQueryParameters
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;

    public Domain.Enums.TaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }

    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }

    public string? Search { get; set; }

    public string? SortBy { get; set; } = "CreatedAt";
    public bool Desc { get; set; } = true;

    public bool AssignedToMe { get; set; } = true;
    public bool CreatedByMe { get; set; } = true;
}