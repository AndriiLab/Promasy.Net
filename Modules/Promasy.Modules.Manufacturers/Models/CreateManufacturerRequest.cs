using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Manufacturers.Interfaces;

namespace Promasy.Modules.Manufacturers.Models;

public record CreateManufacturerRequest(string Name);

internal class CreateManufacturerRequestValidator : AbstractValidator<CreateManufacturerRequest>
{
    public CreateManufacturerRequestValidator(IManufacturersRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium)
            .MustAsync(rules.IsNameUniqueAsync)
            .WithMessage(localizer["Name must be unique"]);
    }
}