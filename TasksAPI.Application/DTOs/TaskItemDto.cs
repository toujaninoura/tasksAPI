namespace TasksAPI.Application.DTOs;

public class TaskItemResponse
{
    public int Id { get; init; }
    public string Titre { get; init; } = string.Empty;
    public string Statut { get; init; } = string.Empty;
}

public class CreateTaskItemRequest
{
    public string Titre { get; init; } = string.Empty;
    public string Statut { get; init; } = "todo";
}

public class UpdateTaskItemRequest
{
    public string? Titre { get; init; }
    public string? Statut { get; init; }
}
