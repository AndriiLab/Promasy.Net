using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Models;

public record CreateDepartmentRequest(string Name, int OrganizationId);

internal class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentRequestValidator(IDepartmentsRules rules, IOrganizationsRules organizationsRules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);
        
        RuleFor(r => r.OrganizationId)
            .MustAsync(organizationsRules.IsExistAsync)
            .WithMessage(localizer["Organization does not exist"]);

        RuleFor(_ => _)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.OrganizationId, t))
            .WithMessage(localizer["Name must be unique"]);
    }
}