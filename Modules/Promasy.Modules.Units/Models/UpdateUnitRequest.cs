using FluentValidation;
using Promasy.Core.Persistence;
using Promasy.Modules.Units.Interfaces;

namespace Promasy.Modules.Units.Models;

public record UpdateUnitRequest(int Id, string Name);

internal class UpdateUnitRequestValidator : AbstractValidator<UpdateUnitRequest>
{
    public UpdateUnitRequestValidator(IUnitsRules rules)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.Id)
            .MustAsync(rules.IsExistAsync)
            .WithMessage("Unit not exist")
            .MustAsync(rules.IsEditableAsync)
            .WithMessage("You cannot edit this unit");
        
        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, t))
            .WithMessage("Name must be unique");
    }
}