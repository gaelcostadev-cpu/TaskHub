using TasksService.Contracts;
using TasksService.Domain.Entities;

namespace TasksService.Mappings;

public static class TaskMappings
{
    public static TaskResponse ToResponse(this TaskItem task)
    {
        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Priority = task.Priority,
            Status = task.Status,
            CreatedByUserId = task.CreatedByUserId,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }

    public static CommentResponse ToResponse(this TaskComment comment)
    {
        return new CommentResponse
        {
            Id = comment.Id,
            AuthorUserId = comment.AuthorUserId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt
        };
    }

    public static TaskHistoryResponse ToResponse(this TaskHistory history)
    {
        return new TaskHistoryResponse
        {
            PropertyName = history.PropertyName,
            OldValue = history.OldValue,
            NewValue = history.NewValue,
            ChangedByUserId = history.ChangedByUserId,
            ChangedAt = history.ChangedAt
        };
    }
}