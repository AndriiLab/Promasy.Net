using FluentValidation;
using Promasy.Core.Persistence;
using Promasy.Modules.Manufacturers.Interfaces;

namespace Promasy.Modules.Manufacturers.Models;

public record UpdateManufacturerRequest(int Id, string Name);

internal class UpdateManufacturerRequestValidator : AbstractValidator<UpdateManufacturerRequest>
{
    public UpdateManufacturerRequestValidator(IManufacturersRules rules)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldLarge);

        RuleFor(r => r.Id)
            .MustAsync(rules.IsExistAsync)
            .WithMessage("Manufacturer not exist")
            .MustAsync(rules.IsEditableAsync)
            .WithMessage("You cannot edit this manufacturer");
        
        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, t))
            .WithMessage("Name must be unique");
    }
}