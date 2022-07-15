using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Models;

public record UpdateDepartmentRequest(int Id, string Name, int OrganizationId);

internal class UpdateDepartmentRequestValidator : AbstractValidator<UpdateDepartmentRequest>
{
    public UpdateDepartmentRequestValidator(IDepartmentRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r)
            .Must(r => rules.IsEditable(r.Id))
            .WithMessage(localizer["You cannot perform this action"])
            .MustAsync((r, t) => rules.IsExistsAsync(r.Id, t))
            .WithMessage(localizer["Item not exist"]);
            
        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, t))
            .WithName(nameof(UpdateDepartmentRequest.Name))
            .WithMessage(localizer["Name must be unique"]);
    }
}