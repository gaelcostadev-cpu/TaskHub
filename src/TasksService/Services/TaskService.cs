using Microsoft.EntityFrameworkCore;
using TasksService.Contracts;
using TasksService.Domain.Entities;
using TasksService.Infrastructure;

namespace TasksService.Services;

public class TaskService : ITaskService
{
    private readonly TasksDbContext _context;

    public TaskService(TasksDbContext context)
    {
        _context = context;
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, Guid userId)
    {
        var task = new TaskItem(
            request.Title,
            request.Description,
            request.DueDate,
            request.Priority,
            userId
        );

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return MapToResponse(task);
    }

    public async Task<TaskResponse?> GetByIdAsync(Guid id, Guid userId)
    {
        var task = await _context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.CreatedByUserId == userId);

        return task is null ? null : MapToResponse(task);
    }

    public async Task<(IEnumerable<TaskResponse> Items, int TotalCount)> GetPagedAsync(int page, int size, Guid userId)
    {
        var query = _context.Tasks
            .Where(t => t.CreatedByUserId == userId)
            .OrderByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync();

        page = (page <= 0) ? 1 : page;
        size = (size > 0 && size <= 100) ? size : 50;

        var tasks = await query
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync();

        return (tasks.Select(MapToResponse), totalCount);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateTaskRequest request, Guid userId)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.CreatedByUserId == userId);

        if (task is null)
            return false;

        task.Update(
            request.Title,
            request.Description,
            request.DueDate,
            request.Priority,
            request.Status
        );

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.CreatedByUserId == userId);

        if (task is null)
            return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return true;
    }

    private static TaskResponse MapToResponse(TaskItem task)
    {
        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Priority = task.Priority,
            Status = task.Status,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}