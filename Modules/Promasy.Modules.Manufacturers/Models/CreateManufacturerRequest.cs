using FluentValidation;
using Promasy.Core.Persistence;
using Promasy.Modules.Manufacturers.Interfaces;

namespace Promasy.Modules.Manufacturers.Models;

public record CreateManufacturerRequest(string Name);

internal class CreateManufacturerRequestValidator : AbstractValidator<CreateManufacturerRequest>
{
    public CreateManufacturerRequestValidator(IManufacturersRules rules)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium)
            .MustAsync(rules.IsNameUniqueAsync)
            .WithMessage("Name must be unique");
    }
}