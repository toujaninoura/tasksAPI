using AutoMapper;
using TasksAPI.Application.DTOs;
using TasksAPI.Domain.Entities;

namespace TasksAPI.Application.Mappings;

public class TaskItemProfile : Profile
{
    public TaskItemProfile()
    {
        CreateMap<TaskItem, TaskItemResponse>();
    }
}
