using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Models;

public record UpdateDepartmentRequest(int Id, string Name, int OrganizationId) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

internal class UpdateDepartmentRequestValidator : AbstractPermissionsValidator<UpdateDepartmentRequest>
{
    public UpdateDepartmentRequestValidator(IDepartmentRules rules, IStringLocalizer<SharedResource> localizer, IUserContext userContext)
        : base(rules, userContext, localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsExistsAsync(r.Id, t))
            .WithMessage(localizer["Item not exist"]);
            
        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, t))
            .WithName(nameof(UpdateDepartmentRequest.Name))
            .WithMessage(localizer["Name must be unique"]);
    }
}