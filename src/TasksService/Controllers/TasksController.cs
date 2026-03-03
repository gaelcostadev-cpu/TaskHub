using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TasksService.Contracts;
using TasksService.Domain.Enums;
using TasksService.Services;

namespace TasksService.Controllers
{

    [ApiController]
    [Route("tasks")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] TaskQueryParameters query)
        {
            var userId = GetUserId();

            var result = await _taskService.GetPagedAsync(query, userId);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = GetUserId();

            var task = await _taskService.GetByIdAsync(id, userId);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
        {
            var userId = GetUserId();

            var task = await _taskService.CreateAsync(request, userId);

            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request)
        {
            var userId = GetUserId();

            var updated = await _taskService.UpdateAsync(id, request, userId);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();

            var deleted = await _taskService.DeleteAsync(id, userId);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpPost("{id:guid}/assign/{assignedUserId:guid}")]
        public async Task<IActionResult> Assign(Guid id, Guid assignedUserId)
        {
            var userId = GetUserId();

            var result = await _taskService.AssignUserAsync(id, assignedUserId, userId);

            return result switch
            {
                AssignUserResult.Success => NoContent(),
                AssignUserResult.TaskNotFound => NotFound(),
                AssignUserResult.NotAllowed => Forbid(),
                AssignUserResult.AlreadyAssigned => Conflict("User already assigned."),
                _ => StatusCode(500)
            };
        }
        
        [HttpPost("{id:guid}/comments")]
        public async Task<IActionResult> AddComment(Guid id, [FromBody] CreateCommentRequest request)
        {
            var userId = GetUserId();

            var result = await _taskService.AddCommentAsync(id, userId, request.Content);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{id:guid}/comments")]
        public async Task<IActionResult> GetComments( Guid id, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var userId = GetUserId();

            var result = await _taskService.GetCommentsAsync(id, page, size, userId);

            return Ok(result);
        }

        private Guid GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (claim == null)
                throw new UnauthorizedAccessException("UserId claim not found.");

            return Guid.Parse(claim);
        }

        [HttpGet("{id:guid}/history")]
        public async Task<IActionResult> GetHistory(Guid id)
        {
            var userId = GetUserId();

            var result = await _taskService.GetHistoryAsync(id, userId);

            return Ok(result);
        }
    }
}