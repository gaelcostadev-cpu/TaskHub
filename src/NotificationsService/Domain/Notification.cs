public class Notification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Message { get; set; } = null!;

    public bool IsRead { get; set; }

    public Guid? EventId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}