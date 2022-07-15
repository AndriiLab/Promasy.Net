using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Rules;
using Promasy.Modules.Suppliers.Interfaces;

namespace Promasy.Modules.Suppliers.Models;

public record UpdateSupplierRequest(int Id, string Name, string? Comment, string? Phone);

internal class UpdateManufacturerRequestValidator : AbstractValidator<UpdateSupplierRequest>
{
    public UpdateManufacturerRequestValidator(ISupplierRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);
        
        RuleFor(r => r.Comment)
            .MaximumLength(PersistenceConstant.FieldLarge);
        
        RuleFor(r => r.Phone)
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.Id)
            .MustAsync(rules.IsExistsAsync)
            .WithMessage(localizer["Item not exist"])
            .MustAsync(rules.IsEditableAsync)
            .WithMessage(localizer["You cannot perform this action"]);
        
        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, t))
            .WithName(nameof(UpdateSupplierRequest.Name))
            .WithMessage(localizer["Name must be unique"]);
    }
}