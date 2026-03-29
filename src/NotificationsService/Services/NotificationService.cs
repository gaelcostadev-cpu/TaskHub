using NotificationsService.Infrastructure;
using NotificationsService.Realtime;
using Shared.Contracts.Events;
using System.Text.Json;

namespace NotificationsService.Services;

public class NotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly NotificationRepository _repository;
    private readonly NotificationDispatcher _dispatcher;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public NotificationService(
        ILogger<NotificationService> logger,
        NotificationRepository repository,
        NotificationDispatcher dispatcher)
    {
        _logger = logger;
        _repository = repository;
        _dispatcher = dispatcher;
    }

    private async Task CreateNotification(Guid userId, string message, Guid? eventId = null)
    {
        if (eventId.HasValue)
        {
            if (await _repository.ExistsByEvent(eventId.Value)) return;
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

        await _dispatcher.SendToUser(userId, new
        {
            id = notification.Id,
            message = notification.Message,
            createdAt = notification.CreatedAt
        });
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

        try 
        {
            switch (routingKey)
            {
                case "task.created":
                    await HandleEvent<TaskCreatedEvent>(message, HandleTaskCreated);
                    break;

                case "task.updated":
                    await HandleEvent<TaskUpdatedEvent>(message, HandleTaskUpdated);
                    break;

                case "task.assigned":
                    await HandleEvent<TaskAssignedEvent>(message, HandleTaskAssigned);
                    break;

                case "comment.created":
                    await HandleEvent<CommentCreatedEvent>(message, HandleCommentCreated);
                    break;

                default:
                    routingLogger.LogWarning("Unhandled event: {RoutingKey}", routingKey);
                    break;
            }
        } catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event {RoutingKey} - Payload: {Payload}", routingKey, message);
            throw;
        }
    }

    private async Task HandleEvent<T>(string message, Func<T, Guid?, Task> handler)
    {
        var envelope = JsonSerializer.Deserialize<EventEnvelope<T>>(message, JsonOptions);

        if (envelope != null)
        {
            if (envelope.Data != null)
            {
                await handler(envelope.Data, envelope.EventId);
            }
            else _logger.LogError("Invalid event payload: {Message}", message);
        }
        else _logger.LogError("Failed to deserialize event envelope: {Message}", message);

        return;
    }

    private Task HandleTaskCreated(TaskCreatedEvent evt, Guid? eventId)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Task created: {TaskId}", evt.TaskId);
        }
        return Task.CompletedTask;
    }

    private Task HandleTaskUpdated(TaskUpdatedEvent evt, Guid? eventId)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Task updated: {TaskId}", evt.TaskId);
        }
        return Task.CompletedTask;
    }

    private async Task HandleTaskAssigned(TaskAssignedEvent evt, Guid? eventId)
    {
        await CreateNotification(
            evt.AssignedUserId,
            $"You were assigned to task {evt.TaskId}",
            eventId
        );
    }

    private async Task HandleCommentCreated(CommentCreatedEvent evt, Guid? eventId)
    {
        await CreateNotification(
            evt.AuthorUserId,
            $"New comment on task {evt.TaskId}"
        );
    }

}