using System.Collections.Concurrent;

namespace NotificationsService.Realtime;

public class UserConnectionManager
{
    private readonly ConcurrentDictionary<string, HashSet<string>> _connections = new();

    public void AddConnection(string userId, string connectionId)
    {
        var connections = _connections.GetOrAdd(userId, _ => new HashSet<string>());

        lock (connections)
        {
            connections.Add(connectionId);
        }
    }

    public void RemoveConnection(string userId, string connectionId)
    {
        if (!_connections.TryGetValue(userId, out var connections))
            return;

        lock (connections)
        {
            connections.Remove(connectionId);

            if (connections.Count == 0)
                _connections.TryRemove(userId, out _);
        }
    }

    public IReadOnlyCollection<string> GetConnections(string userId)
    {
        if (_connections.TryGetValue(userId, out var connections))
            return connections;

        return Array.Empty<string>();
    }
}