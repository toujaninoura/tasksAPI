namespace TasksAPI.Domain.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Titre { get; private set; } = string.Empty;
    public string Statut { get; private set; } = "todo";

    private TaskItem() { }

    public TaskItem(string titre, string statut = "todo")
    {
        Titre = titre;
        Statut = statut;
    }

    public TaskItem WithTitre(string titre) => new TaskItem(titre, Statut) { Id = Id };

    public TaskItem WithStatut(string statut) => new TaskItem(Titre, statut) { Id = Id };
}
