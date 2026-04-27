using AutoMapper;
using TasksAPI.Application.Common;
using TasksAPI.Application.DTOs;
using TasksAPI.Application.Interfaces;
using TasksAPI.Domain.Entities;
using TasksAPI.Domain.Exceptions;

namespace TasksAPI.Application.Services;

public class TaskItemService : ITaskItemService
{
    private readonly ITaskItemRepository _repository;
    private readonly IMapper _mapper;

    public TaskItemService(ITaskItemRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<TaskItemResponse>> GetAllAsync(int page, int pageSize)
    {
        var all = await _repository.GetAllAsync();
        var totalCount = all.Count();
        var items = all
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => _mapper.Map<TaskItemResponse>(t));

        return new PagedResult<TaskItemResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<TaskItemResponse?> GetByIdAsync(int id)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task is null)
            throw new NotFoundException(nameof(TaskItem), id);

        return _mapper.Map<TaskItemResponse>(task);
    }

    public async Task<TaskItemResponse> CreateAsync(CreateTaskItemRequest request)
    {
        var task = new TaskItem(request.Titre, request.Statut);
        var created = await _repository.AddAsync(task);
        return _mapper.Map<TaskItemResponse>(created);
    }

    public async Task<TaskItemResponse> UpdateAsync(int id, UpdateTaskItemRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            throw new NotFoundException(nameof(TaskItem), id);

        var updated = existing.WithStatut(request.Statut).WithTitre(request.Titre);
        var saved = await _repository.UpdateAsync(updated);
        return _mapper.Map<TaskItemResponse>(saved);
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            throw new NotFoundException(nameof(TaskItem), id);

        await _repository.DeleteAsync(id);
    }
}
