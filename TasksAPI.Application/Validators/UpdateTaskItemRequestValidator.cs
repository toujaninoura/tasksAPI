using FluentValidation;
using TasksAPI.Application.DTOs;

namespace TasksAPI.Application.Validators;

public class UpdateTaskItemRequestValidator : AbstractValidator<UpdateTaskItemRequest>
{
    private static readonly string[] ValidStatuts = ["todo", "in-progress", "done"];

    public UpdateTaskItemRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Titre is not null || x.Statut is not null)
            .WithMessage("Au moins un champ (titre ou statut) doit être fourni.");

        When(x => x.Titre is not null, () =>
        {
            RuleFor(x => x.Titre)
                .NotEmpty().WithMessage("Le titre ne peut pas être vide.")
                .MaximumLength(100).WithMessage("Le titre ne peut pas dépasser 100 caractères.");
        });

        When(x => x.Statut is not null, () =>
        {
            RuleFor(x => x.Statut)
                .Must(s => ValidStatuts.Contains(s))
                .WithMessage($"Le statut doit être l'une des valeurs suivantes : {string.Join(", ", ValidStatuts)}.");
        });
    }
}
