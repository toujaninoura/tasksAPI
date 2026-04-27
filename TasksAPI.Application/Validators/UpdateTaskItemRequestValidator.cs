using FluentValidation;
using TasksAPI.Application.DTOs;

namespace TasksAPI.Application.Validators;

public class UpdateTaskItemRequestValidator : AbstractValidator<UpdateTaskItemRequest>
{
    private static readonly string[] ValidStatuts = ["todo", "in-progress", "done"];

    public UpdateTaskItemRequestValidator()
    {
        RuleFor(x => x.Titre)
            .NotEmpty().WithMessage("Le titre est obligatoire.")
            .MaximumLength(200).WithMessage("Le titre ne peut pas dépasser 200 caractères.");

        RuleFor(x => x.Statut)
            .NotEmpty().WithMessage("Le statut est obligatoire.")
            .Must(s => ValidStatuts.Contains(s))
            .WithMessage($"Le statut doit être l'une des valeurs suivantes : {string.Join(", ", ValidStatuts)}.");
    }
}
