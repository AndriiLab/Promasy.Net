using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Rules;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Models;

public record CreateDepartmentRequest(string Name, int OrganizationId);

internal class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentRequestValidator(IDepartmentRules rules, IOrganizationRules organizationRules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);
        
        RuleFor(r => r.OrganizationId)
            .MustAsync(organizationRules.IsExistsAsync)
            .WithMessage(localizer["Organization does not exist"]);

        RuleFor(_ => _)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.OrganizationId, t))
            .WithName(nameof(CreateDepartmentRequest.Name))
            .WithMessage(localizer["Name must be unique"]);
    }
}