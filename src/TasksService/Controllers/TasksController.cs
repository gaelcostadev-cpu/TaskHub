using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TasksService.Contracts;
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
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var userId = GetUserId();

            var result = await _taskService.GetPagedAsync(page, size, userId);

            return Ok(new
            {
                result.TotalCount,
                result.Items
            });
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

        private Guid GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (claim == null)
                throw new UnauthorizedAccessException("UserId claim not found.");

            return Guid.Parse(claim);
        }
    }
}