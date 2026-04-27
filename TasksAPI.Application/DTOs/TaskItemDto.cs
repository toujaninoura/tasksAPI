using System.ComponentModel.DataAnnotations;

namespace TasksAPI.Application.DTOs;

public class TaskItemResponse
{
    public int Id { get; init; }
    public string Titre { get; init; } = string.Empty;
    public string Statut { get; init; } = string.Empty;
}

public class CreateTaskItemRequest
{
    [Required(ErrorMessage = "Le titre est obligatoire.")]
    [MaxLength(100, ErrorMessage = "Le titre ne peut pas dépasser 100 caractères.")]
    public string Titre { get; init; } = string.Empty;
    public string Statut { get; init; } = "todo";
}

public class UpdateTaskItemRequest
{
    public string? Titre { get; init; }
    public string? Statut { get; init; }
}
