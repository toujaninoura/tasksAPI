using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using TasksAPI.Application.DTOs;
using TasksAPI.Application.Validators;

namespace TasksAPI.Tests.Validators;

[TestFixture]
public class CreateTaskItemRequestValidatorTests
{
    private CreateTaskItemRequestValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new CreateTaskItemRequestValidator();
    }

    // Issue #7 - Retourne 400 si titre est absent ou vide
    [Test]
    public void Should_have_error_when_titre_is_empty()
    {
        var request = new CreateTaskItemRequest { Titre = string.Empty, Statut = "todo" };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Titre)
            .WithErrorMessage("Le titre est obligatoire.");
    }

    // Issue #7 - Retourne 400 si titre depasse 100 caracteres
    [Test]
    public void Should_have_error_when_titre_exceeds_100_characters()
    {
        var request = new CreateTaskItemRequest { Titre = new string('a', 101), Statut = "todo" };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Titre)
            .WithErrorMessage("Le titre ne peut pas dépasser 100 caractères.");
    }

    // Issue #7 - Titre valide de 100 caracteres passe la validation
    [Test]
    public void Should_not_have_error_when_titre_is_exactly_100_characters()
    {
        var request = new CreateTaskItemRequest { Titre = new string('a', 100), Statut = "todo" };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Titre);
    }

    // Issue #7 - Titre obligatoire : message d erreur clair
    [Test]
    public void Should_have_clear_error_message_when_titre_is_null()
    {
        var request = new CreateTaskItemRequest { Titre = null!, Statut = "todo" };
        var result = _validator.TestValidate(request);
        result.Errors.Should().Contain(e => e.PropertyName == "Titre");
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("obligatoire"));
    }
}
