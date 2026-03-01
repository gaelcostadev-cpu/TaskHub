using Microsoft.EntityFrameworkCore;
using TasksService.Domain.Entities;

namespace TasksService.Infrastructure;

public class TasksDbContext : DbContext
{
    public TasksDbContext(DbContextOptions<TasksDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskAssignment> TaskAssignments { get; set; }

    public DbSet<TaskItem> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskAssignment>()
        .HasOne(a => a.Task)
        .WithMany(t => t.Assignments)
        .HasForeignKey(a => a.TaskId);

        var taskBuilder = modelBuilder.Entity<TaskItem>();

        taskBuilder.ToTable("tasks");

        taskBuilder.HasKey(t => t.Id);

        taskBuilder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        taskBuilder.Property(t => t.Description)
            .HasMaxLength(2000);

        taskBuilder.Property(t => t.Priority)
            .HasConversion<string>() // enum como texto
            .IsRequired();

        taskBuilder.Property(t => t.Status)
            .HasConversion<string>() // enum como texto
            .IsRequired();

        taskBuilder.Property(t => t.CreatedAt)
            .IsRequired();

        taskBuilder.Property(t => t.UpdatedAt);

        taskBuilder.Property(t => t.CreatedByUserId)
            .IsRequired();

        taskBuilder.HasIndex(t => t.CreatedByUserId);
        taskBuilder.HasIndex(t => t.Status);
        taskBuilder.HasIndex(t => t.Priority);
        taskBuilder.HasIndex(t => t.DueDate);
        taskBuilder.HasIndex(t => t.CreatedAt);
    }
}