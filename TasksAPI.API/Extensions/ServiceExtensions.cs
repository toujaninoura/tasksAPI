using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using TasksAPI.Application.Interfaces;
using TasksAPI.Application.Mappings;
using TasksAPI.Application.Services;
using TasksAPI.Application.Validators;
using TasksAPI.Infrastructure.Data;
using TasksAPI.Infrastructure.Repositories;

namespace TasksAPI.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<ITaskItemService, TaskItemService>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TaskItemProfile>();
        });
        services.AddSingleton(mapperConfig.CreateMapper());

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<CreateTaskItemRequestValidator>();

        return services;
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new()
            {
                Title = "TasksAPI",
                Version = "v1",
                Description = "API de gestion des taches"
            });
        });

        return services;
    }
}
