using FluentValidation;
using Promasy.Core.Persistence;
using Promasy.Modules.Suppliers.Interfaces;

namespace Promasy.Modules.Suppliers.Models;

public record UpdateSupplierRequest(int Id, string Name, string? Comment, string? Phone);

internal class UpdateManufacturerRequestValidator : AbstractValidator<UpdateSupplierRequest>
{
    public UpdateManufacturerRequestValidator(ISuppliersRules rules)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);
        
        RuleFor(r => r.Comment)
            .MaximumLength(PersistenceConstant.FieldLarge);
        
        RuleFor(r => r.Phone)
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.Id)
            .MustAsync(rules.IsExistAsync)
            .WithMessage("Supplier not exist")
            .MustAsync(rules.IsEditableAsync)
            .WithMessage("You cannot edit this supplier");
        
        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, t))
            .WithMessage("Name must be unique");
    }
}