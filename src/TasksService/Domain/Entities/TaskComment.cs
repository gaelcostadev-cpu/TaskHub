namespace TasksService.Domain.Entities;

public class TaskComment
{
    public Guid Id { get; private set; }

    public Guid TaskId { get; private set; }
    public TaskItem Task { get; private set; } = null!;

    public Guid AuthorUserId { get; private set; }

    public string Content { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    private TaskComment() { } // EF

    public TaskComment(Guid taskId, Guid authorUserId, string content)
    {
        Id = Guid.NewGuid();
        TaskId = taskId;
        AuthorUserId = authorUserId;
        Content = content;
        CreatedAt = DateTime.UtcNow;
    }
}