using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Models;

public record CreateSubDepartmentRequest(string Name, int DepartmentId, int OrganizationId);

internal class CreateSubDepartmentRequestValidator : AbstractValidator<CreateSubDepartmentRequest>
{
    public CreateSubDepartmentRequestValidator(ISubDepartmentsRules rules, IOrganizationsRules organizationsRules,
        IDepartmentsRules departmentsRules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);
        
        RuleFor(r => r.OrganizationId)
            .MustAsync(organizationsRules.IsExistAsync)
            .WithMessage(localizer["Organization does not exist"]);

        RuleFor(_ => _)
            .MustAsync((r, t) => departmentsRules.IsExistAsync(r.DepartmentId, r.OrganizationId, t))
            .WithMessage(localizer["Department does not exist"])
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.DepartmentId, r.OrganizationId, t))
            .WithMessage(localizer["Name must be unique"]);
    }
}