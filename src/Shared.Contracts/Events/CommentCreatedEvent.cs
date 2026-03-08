namespace Shared.Contracts.Events;

public record CommentCreatedEvent
(
    Guid TaskId,
    Guid CommentId,
    Guid AuthorUserId,
    string Content,
    DateTime CreatedAt
);