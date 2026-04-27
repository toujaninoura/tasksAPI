using Microsoft.AspNetCore.Mvc;
using TasksAPI.Application.Common;
using TasksAPI.Application.DTOs;
using TasksAPI.Application.Interfaces;

namespace TasksAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskItemsController : ControllerBase
{
    private readonly ITaskItemService _service;
    private readonly ILogger<TaskItemsController> _logger;

    public TaskItemsController(ITaskItemService service, ILogger<TaskItemsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TaskItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Getting all tasks - page: {Page}, pageSize: {PageSize}", page, pageSize);
        var result = await _service.GetAllAsync(page, pageSize);
        return Ok(ApiResponse<PagedResult<TaskItemResponse>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TaskItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("Getting task by id: {Id}", id);
        var task = await _service.GetByIdAsync(id);
        return Ok(ApiResponse<TaskItemResponse>.Ok(task!));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TaskItemResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTaskItemRequest request)
    {
        _logger.LogInformation("Creating new task: {Titre}", request.Titre);
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<TaskItemResponse>.Ok(created, "Tache creee avec succes."));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TaskItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskItemRequest request)
    {
        _logger.LogInformation("Updating task id: {Id}", id);
        var updated = await _service.UpdateAsync(id, request);
        return Ok(ApiResponse<TaskItemResponse>.Ok(updated, "Tache mise a jour avec succes."));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting task id: {Id}", id);
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
