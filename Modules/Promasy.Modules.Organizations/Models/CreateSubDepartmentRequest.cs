using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Models;

public record CreateSubDepartmentRequest(string Name, int DepartmentId, int OrganizationId) : IRequestWithPermissionValidation
{
    public int GetId() => DepartmentId;
}

internal class CreateSubDepartmentRequestValidator : AbstractPermissionsValidator<CreateSubDepartmentRequest>
{
    public CreateSubDepartmentRequestValidator(ISubDepartmentRules rules, IOrganizationRules organizationRules,
        IDepartmentRules departmentRules, IStringLocalizer<SharedResource> localizer, IUserContext userContext)
        : base(departmentRules, userContext, localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);
        
        RuleFor(r => r.OrganizationId)
            .MustAsync(organizationRules.IsExistsAsync)
            .WithMessage(localizer["Organization does not exist"]);

        RuleFor(_ => _)
            .MustAsync((r, t) => departmentRules.IsExistsAsync(r.DepartmentId, t))
            .WithName(nameof(CreateSubDepartmentRequest.DepartmentId))
            .WithMessage(localizer["Department does not exist"]);

        RuleFor(_ => _)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.DepartmentId, t))
            .WithName(nameof(CreateSubDepartmentRequest.Name))
            .WithMessage(localizer["Name must be unique"]);
    }
}