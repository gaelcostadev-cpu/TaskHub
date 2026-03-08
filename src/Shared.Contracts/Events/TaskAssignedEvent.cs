namespace Shared.Contracts.Events;

public record TaskAssignedEvent
(
    Guid TaskId,
    Guid AssignedUserId,
    Guid AssignedByUserId,
    DateTime AssignedAt
);