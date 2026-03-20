using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NotificationsService.Infrastructure;

public class NotificationsDbContextFactory
    : IDesignTimeDbContextFactory<NotificationsDbContext>
{
    public NotificationsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<NotificationsDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=taskhub_notifications;" +
            "Username=postgres;Password=postgres")
            .Options;

        return new NotificationsDbContext(options);
    }
}