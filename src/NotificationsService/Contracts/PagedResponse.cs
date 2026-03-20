namespace NotificationsService.Contracts;

public record PagedResponse<T>
(
    IEnumerable<T> Data,
    int Page,
    int Size,
    int Total
);