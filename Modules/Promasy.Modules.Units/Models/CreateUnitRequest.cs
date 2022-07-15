using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Units.Interfaces;

namespace Promasy.Modules.Units.Models;

public record CreateUnitRequest(string Name);

internal class CreateUnitRequestValidator : AbstractValidator<CreateUnitRequest>
{
    public CreateUnitRequestValidator(IUnitRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium)
            .MustAsync(rules.IsNameUniqueAsync)
            .WithMessage(localizer["Name must be unique"]);
    }
}