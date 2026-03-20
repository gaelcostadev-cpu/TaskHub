using Microsoft.EntityFrameworkCore;
using NotificationsService.Models;

namespace NotificationsService.Infrastructure;

public class NotificationRepository
{
    private readonly NotificationsDbContext _db;

    public NotificationRepository(NotificationsDbContext db)
    {
        _db = db;
    }

    public Task<int> GetUnreadCount(Guid userId)
    {
        return _db.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public Task<bool> ExistsByEvent(Guid eventId)
    {
        return _db.Notifications
            .AnyAsync(n => n.EventId == eventId);
    }

    public async Task MarkManyAsRead(Guid userId)
    {
        await _db.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(n => n.IsRead, true));
    }
    public async Task AddAsync(Notification notification)
    {
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Notification>> GetByUser(Guid userId, int page = 1, int pageSize = 20)
    {
        return await _db.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task MarkAsRead(Guid id)
    {
        var notification = await _db.Notifications.FindAsync(id);

        if (notification == null)
            return;

        notification.IsRead = true;

        await _db.SaveChangesAsync();
    }

    public async Task<(List<Notification>, int)> GetPagedByUser(
    Guid userId,
    int page,
    int pageSize)
    {
        var query = _db.Notifications
            .Where(n => n.UserId == userId);

        var total = await query.CountAsync();

        var data = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (data, total);
    }
}