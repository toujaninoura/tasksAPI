using TasksAPI.Domain.Entities;

namespace TasksAPI.Application.Interfaces;

public interface ITaskItemRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(int id);
    Task<TaskItem> AddAsync(TaskItem task);
    Task<TaskItem> UpdateAsync(TaskItem task);
    Task DeleteAsync(int id);
}
