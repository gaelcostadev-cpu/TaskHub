using NotificationsService.Contracts;
using NotificationsService.Infrastructure;

namespace NotificationsService.Endpoints;

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this WebApplication app)
    {
        app.MapGet(
            "/notifications/{userId}",
            async (
                Guid userId,
                int page,
                int size,
                NotificationRepository repo) =>
            {
                var (notifications, total) = await repo.GetPagedByUser(userId, page, size);

                var response = new PagedResponse<NotificationResponse>(
                    notifications.Select(n =>
                        new NotificationResponse(
                            n.Id,
                            n.Message,
                            n.IsRead,
                            n.CreatedAt
                        )
                    ),
                    page,
                    size,
                    total
                );

                return Results.Ok(response);
            });

        app.MapGet(
            "/notifications/{userId}/unread-count",
            async (Guid userId, NotificationRepository repo) =>
            {
                var count = await repo.GetUnreadCount(userId);
                return Results.Ok(new { count });
            });

        app.MapPost(
            "/notifications/{userId}/read-all",
            async (Guid userId, NotificationRepository repo) =>
            {
                await repo.MarkManyAsRead(userId);
                return Results.Ok();
            });

        app.MapPost(
            "/notifications/{id}/read",
            async (Guid id, NotificationRepository repo) =>
            {
                await repo.MarkAsRead(id);
                return Results.Ok();
            });
    }
}