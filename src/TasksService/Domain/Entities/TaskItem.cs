using TasksService.Domain.Enums;

namespace TasksService.Domain.Entities
{

    public class TaskItem
    {
        public Guid Id { get; private set; }

        public string Title { get; private set; } = null!;

        public string? Description { get; private set; }

        public DateTime? DueDate { get; private set; }

        public TaskPriority Priority { get; private set; }

        public Enums.TaskStatus Status { get; private set; }

        public Guid CreatedByUserId { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime? UpdatedAt { get; private set; }

        private TaskItem() { }

        public TaskItem(
            string title,
            string? description,
            DateTime? dueDate,
            TaskPriority priority,
            Guid createdByUserId)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            Status = Enums.TaskStatus.TODO;
            CreatedByUserId = createdByUserId;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            string title,
            string? description,
            DateTime? dueDate,
            TaskPriority priority,
            Enums.TaskStatus status)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeStatus(Enums.TaskStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}