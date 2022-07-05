using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Manufacturers.Interfaces;

namespace Promasy.Modules.Manufacturers.Models;

public record UpdateManufacturerRequest(int Id, string Name);

internal class UpdateManufacturerRequestValidator : AbstractValidator<UpdateManufacturerRequest>
{
    public UpdateManufacturerRequestValidator(IManufacturersRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.Id)
            .MustAsync(rules.IsExistAsync)
            .WithMessage(localizer["Manufacturer not exist"])
            .MustAsync(rules.IsEditableAsync)
            .WithMessage(localizer["You cannot perform this action"]);
        
        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, t))
            .WithMessage(localizer["Name must be unique"]);
    }
}