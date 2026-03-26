using Microsoft.AspNetCore.SignalR;

namespace NotificationsService.Realtime;

public class NotificationsHub : Hub
{
    private readonly UserConnectionManager _connections;

    public NotificationsHub(UserConnectionManager connections)
    {
        _connections = connections;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"];

        if (!string.IsNullOrWhiteSpace(userId))
        {
            _connections.AddConnection(userId!, Context.ConnectionId);

            await Groups.AddToGroupAsync(Context.ConnectionId, userId!);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"];

        if (!string.IsNullOrWhiteSpace(userId))
        {
            _connections.RemoveConnection(userId!, Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}