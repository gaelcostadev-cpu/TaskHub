namespace TasksService.Contracts;

public sealed class PagedResponse<T>
{
    public int Page { get; set; }
    public int Size { get; set; }
    public int TotalCount { get; set; }
    public IEnumerable<T> Items { get; set; } = [];
}