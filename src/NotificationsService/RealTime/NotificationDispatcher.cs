using Microsoft.AspNetCore.SignalR;

namespace NotificationsService.Realtime;

public class NotificationDispatcher
{
    private readonly IHubContext<NotificationsHub> _hub;

    public NotificationDispatcher(IHubContext<NotificationsHub> hub)
    {
        _hub = hub;
    }

    public async Task SendToUser(Guid userId, object payload)
    {
        await _hub
            .Clients
            .Group(userId.ToString())
            .SendAsync("notification.received", payload);
    }
}