using System;

namespace TasksService.Domain.Entities;

public class TaskAssignment
{
    public Guid Id { get; private set; }

    public Guid TaskId { get; private set; }
    public TaskItem Task { get; private set; } = null!;

    public Guid AssignedUserId { get; private set; }

    public DateTime AssignedAt { get; private set; }

    private TaskAssignment() { } // EF

    public TaskAssignment(Guid taskId, Guid assignedUserId)
    {
        Id = Guid.NewGuid();
        TaskId = taskId;
        AssignedUserId = assignedUserId;
        AssignedAt = DateTime.UtcNow;
    }
}