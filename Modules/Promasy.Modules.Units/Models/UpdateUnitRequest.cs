﻿using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Rules;
using Promasy.Modules.Units.Interfaces;

namespace Promasy.Modules.Units.Models;

public record UpdateUnitRequest(int Id, string Name);

internal class UpdateUnitRequestValidator : AbstractValidator<UpdateUnitRequest>
{
    public UpdateUnitRequestValidator(IUnitsRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.Id)
            .MustAsync(rules.IsExistAsync)
            .WithMessage(localizer["Item not exist"])
            .MustAsync(rules.IsEditableAsync)
            .WithMessage(localizer["You cannot perform this action"]);
        
        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, t))
            .WithMessage(localizer["Name must be unique"]);
    }
}