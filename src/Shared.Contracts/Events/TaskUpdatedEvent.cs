namespace Shared.Contracts.Events;

public record TaskUpdatedEvent
(
    Guid TaskId,
    Guid UpdatedByUserId,
    DateTime UpdatedAt
);