namespace TasksService.Contracts;


public class CommentResponse
{
    public Guid Id { get; set; }
    public Guid AuthorUserId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}