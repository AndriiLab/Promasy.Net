using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Rules;
using Promasy.Modules.Suppliers.Interfaces;

namespace Promasy.Modules.Suppliers.Models;

public record CreateSupplierRequest(string Name, string? Comment, string? Phone);

internal class CreateManufacturerRequestValidator : AbstractValidator<CreateSupplierRequest>
{
    public CreateManufacturerRequestValidator(ISupplierRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium)
            .MustAsync(rules.IsNameUniqueAsync)
            .WithMessage(localizer["Name must be unique"]);
        
        RuleFor(r => r.Comment)
            .MaximumLength(PersistenceConstant.FieldLarge);
        
        RuleFor(r => r.Phone)
            .MaximumLength(PersistenceConstant.FieldMedium);
    }
}