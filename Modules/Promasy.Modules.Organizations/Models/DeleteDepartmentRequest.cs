using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Models;

public record DeleteDepartmentRequest(int Id, int OrganizationId) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

internal class DeleteDepartmentRequestValidator : AbstractPermissionsValidator<DeleteDepartmentRequest>
{
    public DeleteDepartmentRequestValidator(IDepartmentRules rules, IUserContext userContext, IStringLocalizer<SharedResource> localizer) : base(rules, userContext, localizer)
    {
        RuleFor(d => d.Id)
            .MustAsync(async (i, c) => await rules.IsUsedAsync(i, c) == false)
            .WithMessage(localizer["Department has subdepartments"])
            .WithErrorCode(StatusCodes.Status409Conflict.ToString());
    }
}