using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Models;

public record DeleteSubDepartmentRequest(int Id, int OrganizationId, int DepartmentId) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

internal class DeleteSubDepartmentRequestValidator : AbstractPermissionsValidator<DeleteSubDepartmentRequest>
{
    public DeleteSubDepartmentRequestValidator(ISubDepartmentRules rules, IUserContext userContext, IStringLocalizer<SharedResource> localizer)
        : base(rules, userContext, localizer)
    {
        RuleFor(r => r.Id)
            .MustAsync(async (i, c) => await rules.IsUsedAsync(i, c) == false)
            .WithMessage(localizer["Sub-department has employees"])
            .WithErrorCode(StatusCodes.Status409Conflict.ToString());
    }
}