using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Models;

public record CreateDepartmentRequest(string Name, int OrganizationId): IRequestWithPermissionValidation
{
    public int GetId() => OrganizationId;
}

internal class CreateDepartmentRequestValidator : AbstractPermissionsValidator<CreateDepartmentRequest>
{
    public CreateDepartmentRequestValidator(IDepartmentRules rules, IOrganizationRules organizationRules, IStringLocalizer<SharedResource> localizer, IUserContext userContext)
        : base(organizationRules, userContext, localizer)
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