using Microsoft.EntityFrameworkCore;
using TasksService.Contracts;
using TasksService.Domain.Entities;
using TasksService.Domain.Enums;
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
        .FirstOrDefaultAsync(
            t => t.Id == id && 
            (t.CreatedByUserId == userId ||
            t.Assignments.Any(a => a.AssignedUserId == userId))
        );

        return task is null ? null : MapToResponse(task);
    }

    public async Task<PagedResponse<TaskResponse>> GetPagedAsync(TaskQueryParameters parameters, Guid userId)
    {
        var query = _context.Tasks
            .Include(t => t.Assignments)
            .AsQueryable();

        // Segurança
        if (parameters.AssignedToMe && parameters.CreatedByMe)
        {
            query = query.Where(t =>
                t.CreatedByUserId == userId ||
                t.Assignments.Any(a => a.AssignedUserId == userId));
        }
        else if (parameters.AssignedToMe)
        {
            query = query.Where(t =>
                t.Assignments.Any(a => a.AssignedUserId == userId));
        }
        else if (parameters.CreatedByMe)
        {
            query = query.Where(t =>
                t.CreatedByUserId == userId);
        }

        // Filtros
        if (parameters.Status.HasValue)
            query = query.Where(t => t.Status == parameters.Status);

        if (parameters.Priority.HasValue)
            query = query.Where(t => t.Priority == parameters.Priority);

        if (parameters.DueDateFrom.HasValue)
        {
            /// Garantir que as datas sejam comparadas em UTC
            var fromUtc = DateTime.SpecifyKind(
                parameters.DueDateFrom.Value,
                DateTimeKind.Utc);

            query = query.Where(t => t.DueDate >= fromUtc);
        }

        if (parameters.DueDateTo.HasValue)
        {
            /// Garantir que as datas sejam comparadas em UTC
            var toUtc = DateTime.SpecifyKind(
                parameters.DueDateTo.Value,
                DateTimeKind.Utc);

            query = query.Where(t => t.DueDate <= toUtc);
        }


        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLower();
            query = query.Where(t =>
                EF.Functions.ILike(t.Title, $"%{parameters.Search}%"));
        }

        // Ordenação
        query = parameters.SortBy?.ToLower() switch
        {
            "duedate" => parameters.Desc
                ? query.OrderByDescending(t => t.DueDate)
                : query.OrderBy(t => t.DueDate),

            "priority" => parameters.Desc
                ? query.OrderByDescending(t => t.Priority)
                : query.OrderBy(t => t.Priority),

            _ => parameters.Desc
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt)
        };

        var totalCount = await query.CountAsync();

        var page = parameters.Page <= 0 ? 1 : parameters.Page;
        var size = parameters.Size is > 0 and <= 100 ? parameters.Size : 10;

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync();

        return new PagedResponse<TaskResponse>
        {
            Page = page,
            Size = size,
            TotalCount = totalCount,
            Items = items.Select(MapToResponse)
        };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateTaskRequest request, Guid userId)
    {
        var task = await _context.Tasks
        .FirstOrDefaultAsync(
            t => t.Id == id &&
            t.CreatedByUserId == userId
        );

        if (task is null)
            return false;

        var oldTitle = task.Title;
        var oldDescription = task.Description;
        var oldDueDate = task.DueDate;
        var oldPriority = task.Priority;
        var oldStatus = task.Status;

        if (oldTitle != request.Title)
            AddHistory(task.Id, "Title", oldTitle, request.Title, userId);

        if (oldDescription != request.Description)
            AddHistory(task.Id, "Description", oldDescription, request.Description, userId);

        if (oldDueDate != request.DueDate)
        {
            AddHistory(task.Id, "DueDate",
                oldDueDate.HasValue ? oldDueDate.Value.ToString("O") : null,
                request.DueDate.ToString("O"),
                userId);
        }

        if (oldPriority != request.Priority)
        {
            AddHistory(task.Id, "Priority",
                oldPriority.ToString(),
                request.Priority.ToString(),
                userId);
        }

        if (oldStatus != request.Status)
        {
            AddHistory(task.Id, "Status",
                oldStatus.ToString(),
                request.Status.ToString(),
                userId);
        }

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

    public async Task<AssignUserResult> AssignUserAsync(Guid taskId, Guid assignedUserId, Guid requesterId)
    {
        var task = await _context.Tasks
            .Include(t => t.Assignments)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task is null)
            return AssignUserResult.TaskNotFound;

        if (task.CreatedByUserId != requesterId)
            return AssignUserResult.NotAllowed;

        var assigned = task.AssignUser(assignedUserId);

        if (!assigned)
            return AssignUserResult.AlreadyAssigned;

        AddHistory(task.Id, "Assignment", null, assignedUserId.ToString(), requesterId);

        await _context.SaveChangesAsync();

        return AssignUserResult.Success;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync
            (
                t => t.Id == id && t.CreatedByUserId == userId
            );

        if (task is null) return false;

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

    public async Task<PagedResponse<CommentResponse>> GetCommentsAsync(Guid taskId, int page, int size, Guid userId)
    {
        var taskExists = await _context.Tasks
            .AnyAsync(t =>
                t.Id == taskId &&
                (
                    t.CreatedByUserId == userId ||
                    t.Assignments.Any(a => a.AssignedUserId == userId)
                )
            );

        if (!taskExists)
            return new PagedResponse<CommentResponse>
            {
                Page = page,
                Size = size,
                TotalCount = 0,
                Items = Enumerable.Empty<CommentResponse>()
            };

        page = page <= 0 ? 1 : page;
        size = size is > 0 and <= 100 ? size : 10;

        var query = _context.TaskComments
            .Where(c => c.TaskId == taskId)
            .OrderBy(c => c.CreatedAt);

        var total = await query.CountAsync();

        var comments = await query
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync();

        return new PagedResponse<CommentResponse>
        {
            Page = page,
            Size = size,
            TotalCount = total,
            Items = comments.Select(c => new CommentResponse
            {
                Id = c.Id,
                AuthorUserId = c.AuthorUserId,
                Content = c.Content,
                CreatedAt = c.CreatedAt
            })
        };
    }

    public async Task<CommentResponse?> AddCommentAsync(Guid taskId, Guid userId, string content)
    {
        var task = await _context.Tasks
            .Include(t => t.Assignments)
            .FirstOrDefaultAsync(t =>
                t.Id == taskId &&
                (
                    t.CreatedByUserId == userId ||
                    t.Assignments.Any(a => a.AssignedUserId == userId)
                )
            );

        if (task is null)
            return null;

        var comment = new TaskComment(taskId, userId, content);

        _context.TaskComments.Add(comment);

        await _context.SaveChangesAsync();

        return new CommentResponse
        {
            Id = comment.Id,
            AuthorUserId = comment.AuthorUserId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt
        };
    }

    private void AddHistory(Guid taskId, string propertyName, string? oldValue, string? newValue, Guid userId)
    {
        var history = new TaskHistory(
            taskId,
            propertyName,
            oldValue,
            newValue,
            userId
        );

        _context.TaskHistories.Add(history);
    }

    public async Task<IEnumerable<TaskHistoryResponse>> GetHistoryAsync(Guid taskId, Guid userId)
    {
        var taskExists = await _context.Tasks
            .AnyAsync(t =>
                t.Id == taskId &&
                (
                    t.CreatedByUserId == userId ||
                    t.Assignments.Any(a => a.AssignedUserId == userId)
                )
            );

        if (!taskExists)
            return Enumerable.Empty<TaskHistoryResponse>();

        var history = await _context.TaskHistories
            .Where(h => h.TaskId == taskId)
            .OrderByDescending(h => h.ChangedAt)
            .AsNoTracking()
            .ToListAsync();

        return history.Select(h => new TaskHistoryResponse
        {
            PropertyName = h.PropertyName,
            OldValue = h.OldValue,
            NewValue = h.NewValue,
            ChangedByUserId = h.ChangedByUserId,
            ChangedAt = h.ChangedAt
        });
    }

}