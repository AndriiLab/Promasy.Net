using FluentValidation;
using Promasy.Core.Persistence;
using Promasy.Modules.Units.Interfaces;

namespace Promasy.Modules.Units.Models;

public record CreateUnitRequest(string Name);

internal class CreateUnitRequestValidator : AbstractValidator<CreateUnitRequest>
{
    public CreateUnitRequestValidator(IUnitsRules rules)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldLarge)
            .MustAsync(rules.IsNameUniqueAsync)
            .WithMessage("Name must be unique");
    }
}