namespace Shared.Contracts.Events;

public record EventEnvelope<T>
(
    Guid EventId,
    string EventType,
    int Version,
    DateTime OccurredAt,
    T Data
);