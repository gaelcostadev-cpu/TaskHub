namespace TasksService.Domain.Entities;

public class TaskHistory
{
    public Guid Id { get; private set; }
    public Guid TaskId { get; private set; }

    public string PropertyName { get; private set; } = null!;
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }

    public Guid ChangedByUserId { get; private set; }
    public DateTime ChangedAt { get; private set; }

    public TaskItem Task { get; private set; } = null!;

    private TaskHistory() { } 

    public TaskHistory(
        Guid taskId,
        string propertyName,
        string? oldValue,
        string? newValue,
        Guid changedByUserId)
    {
        Id = Guid.NewGuid();
        TaskId = taskId;
        PropertyName = propertyName;
        OldValue = oldValue;
        NewValue = newValue;
        ChangedByUserId = changedByUserId;
        ChangedAt = DateTime.UtcNow;
    }
}