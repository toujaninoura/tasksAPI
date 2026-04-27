using TasksAPI.Application.Common;
using TasksAPI.Application.DTOs;

namespace TasksAPI.Application.Interfaces;

public interface ITaskItemService
{
    Task<PagedResult<TaskItemResponse>> GetAllAsync(int page, int pageSize);
    Task<TaskItemResponse?> GetByIdAsync(int id);
    Task<TaskItemResponse> CreateAsync(CreateTaskItemRequest request);
    Task<TaskItemResponse> UpdateAsync(int id, UpdateTaskItemRequest request);
    Task DeleteAsync(int id);
}
