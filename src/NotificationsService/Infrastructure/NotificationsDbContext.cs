using Microsoft.EntityFrameworkCore;

namespace NotificationsService.Infrastructure;

public class NotificationsDbContext : DbContext
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Notification>(entity =>
        {
            entity.ToTable("notifications");

            entity.HasKey(n => n.Id);

            entity.HasIndex(n => n.UserId);

            entity.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(n => n.UserId)
                .IsRequired();

            entity.Property(n => n.IsRead)
                .HasDefaultValue(false);

            entity.Property(n => n.EventId)
                .IsRequired(false);

            entity.HasIndex(n => n.EventId)
                .IsUnique();

            entity.HasIndex(n => new { n.UserId, n.IsRead });

            entity.Property(n => n.CreatedAt)
                .HasDefaultValueSql("NOW()");
        });
    }
}