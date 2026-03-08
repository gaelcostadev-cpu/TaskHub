namespace Shared.Contracts.Events;

public record TaskCreatedEvent
(
    Guid TaskId,
    string Title,
    Guid CreatedByUserId,
    DateTime CreatedAt
);