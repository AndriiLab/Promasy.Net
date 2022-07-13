﻿using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Rules;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Models;

public record UpdateDepartmentRequest(int Id, string Name, int OrganizationId);

internal class UpdateDepartmentRequestValidator : AbstractValidator<UpdateDepartmentRequest>
{
    public UpdateDepartmentRequestValidator(IDepartmentsRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r)
            .Must(r => rules.IsEditable(r.Id))
            .WithMessage(localizer["You cannot perform this action"])
            .MustAsync((r, t) => rules.IsExistAsync(r.Id, t))
            .WithMessage(localizer["Item not exist"])
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, t))
            .WithMessage(localizer["Name must be unique"]);
    }
}