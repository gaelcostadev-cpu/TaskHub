using System.Text.Json;
using Shared.Contracts.Events;

namespace NotificationsService.Services;

public class NotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(string routingKey, string message)
    {
        switch (routingKey)
        {
            case "task.created":
                var taskCreated = JsonSerializer.Deserialize<TaskCreatedEvent>(message);
                await HandleTaskCreated(taskCreated!);
                break;

            case "task.updated":
                var taskUpdated = JsonSerializer.Deserialize<TaskUpdatedEvent>(message);
                await HandleTaskUpdated(taskUpdated!);
                break;

            case "task.assigned":
                var taskAssigned = JsonSerializer.Deserialize<TaskAssignedEvent>(message);
                await HandleTaskAssigned(taskAssigned!);
                break;

            case "comment.created":
                var commentCreated = JsonSerializer.Deserialize<CommentCreatedEvent>(message);
                await HandleCommentCreated(commentCreated!);
                break;

            default:
                _logger.LogWarning("Unhandled event: {RoutingKey}", routingKey);
                break;
        }
    }

    private Task HandleTaskCreated(TaskCreatedEvent evt)
    {
        _logger.LogInformation("Task created: {TaskId}", evt.TaskId);
        return Task.CompletedTask;
    }

    private Task HandleTaskUpdated(TaskUpdatedEvent evt)
    {
        _logger.LogInformation("Task updated: {TaskId}", evt.TaskId);
        return Task.CompletedTask;
    }

    private Task HandleTaskAssigned(TaskAssignedEvent evt)
    {
        _logger.LogInformation("Task assigned: {TaskId} -> {User}", evt.TaskId, evt.AssignedUserId);
        return Task.CompletedTask;
    }

    private Task HandleCommentCreated(CommentCreatedEvent evt)
    {
        _logger.LogInformation("Comment created: {CommentId}", evt.CommentId);
        return Task.CompletedTask;
    }
}