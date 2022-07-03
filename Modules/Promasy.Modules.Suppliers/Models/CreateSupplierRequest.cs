using FluentValidation;
using Promasy.Core.Persistence;
using Promasy.Modules.Suppliers.Interfaces;

namespace Promasy.Modules.Suppliers.Models;

public record CreateSupplierRequest(string Name, string? Comment, string? Phone);

internal class CreateManufacturerRequestValidator : AbstractValidator<CreateSupplierRequest>
{
    public CreateManufacturerRequestValidator(ISuppliersRules rules)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium)
            .MustAsync(rules.IsNameUniqueAsync)
            .WithMessage("Name must be unique");
        
        RuleFor(r => r.Comment)
            .MaximumLength(PersistenceConstant.FieldLarge);
        
        RuleFor(r => r.Phone)
            .MaximumLength(PersistenceConstant.FieldMedium);
    }
}