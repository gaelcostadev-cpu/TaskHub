namespace NotificationsService.Contracts;

public record NotificationResponse
(
    Guid Id,
    string Message,
    bool IsRead,
    DateTime CreatedAt
);