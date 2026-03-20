using NotificationsService.Infrastructure;
using Shared.Contracts.Events;
using System.Text.Json;

namespace NotificationsService.Services;

public class NotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly NotificationRepository _repository;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public NotificationService(
        ILogger<NotificationService> logger,
        NotificationRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    private async Task CreateNotification(Guid userId, string message, Guid? eventId = null)
    {
        if (eventId.HasValue)
        {
            var exists = await _repository.ExistsByEvent(eventId.Value);

            if (exists)
                return;
        }

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Message = message,
            EventId = eventId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(notification);
    }

    public ILogger Get_logger()
    {
        return _logger;
    }

    public async Task HandleAsync(string routingKey, string message, ILogger routingLogger)
    {
        if (routingLogger.IsEnabled(LogLevel.Information))
        {
            routingLogger.LogInformation(
                "Processing event {RoutingKey}",
                routingKey
            );
        }

        switch (routingKey)
        {
            case "task.created":
                if (routingLogger.IsEnabled(LogLevel.Information))
                {
                    routingLogger.LogInformation(
                        "Processing event {RoutingKey}",
                        routingKey
                    );
                }
                var envelope = JsonSerializer.Deserialize<EventEnvelope<TaskCreatedEvent>>(message, JsonOptions);
                var taskCreated = envelope?.Data;
                await HandleTaskCreated(taskCreated!);
                break;

            case "task.updated":
                if (routingLogger.IsEnabled(LogLevel.Information))
                {
                    routingLogger.LogInformation(
                        "Processing event {RoutingKey}",
                        routingKey
                    );
                }
                var envelope2 = JsonSerializer.Deserialize<EventEnvelope<TaskUpdatedEvent>>(message, JsonOptions);
                var taskUpdated = envelope2?.Data;
                await HandleTaskUpdated(taskUpdated!);
                break;

            case "task.assigned":
                if (routingLogger.IsEnabled(LogLevel.Information))
                {
                    routingLogger.LogInformation(
                        "Processing event {RoutingKey}",
                        routingKey
                    );
                }
                var envelope3 = JsonSerializer.Deserialize<EventEnvelope<TaskAssignedEvent>>(message, JsonOptions);
                
                if (envelope3?.Data == null)
                {
                    _logger.LogError("Invalid TaskAssignedEvent payload: {Message}", message);
                    return;
                }

                await HandleTaskAssigned(envelope3.Data, envelope3.EventId);
                break;

            case "comment.created":
                if (routingLogger.IsEnabled(LogLevel.Information))
                {
                    routingLogger.LogInformation(
                        "Processing event {RoutingKey}",
                        routingKey
                    );
                }
                var envelope4 = JsonSerializer.Deserialize<EventEnvelope<CommentCreatedEvent>>(message, JsonOptions);
                var commentCreated = envelope4?.Data;
                await HandleCommentCreated(commentCreated!);
                break;

            default:
                if (routingLogger.IsEnabled(LogLevel.Warning))
                {
                    routingLogger.LogWarning("Unhandled event: {RoutingKey}", routingKey);
                }
                break;
        }
    }

    private Task HandleTaskCreated(TaskCreatedEvent evt)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Task created: {TaskId}", evt.TaskId);
        }
        return Task.CompletedTask;
    }

    private Task HandleTaskUpdated(TaskUpdatedEvent evt)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Task updated: {TaskId}", evt.TaskId);
        }
        return Task.CompletedTask;
    }

    private async Task HandleTaskAssigned(TaskAssignedEvent evt, Guid eventId)
    {
        await CreateNotification(
            evt.AssignedUserId,
            $"You were assigned to task {evt.TaskId}",
            eventId
        );
    }

    private async Task HandleCommentCreated(CommentCreatedEvent evt)
    {
        await CreateNotification(
            evt.AuthorUserId,
            $"New comment on task {evt.TaskId}"
        );
    }

}