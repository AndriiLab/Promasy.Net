using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Domain.Organizations;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;

namespace Promasy.Modules.Finances.Models;

public record GetFinanceSubDepartmentRequest(int FinanceId, int SubDepartmentId) : IRequestWithPermissionValidation
{
    public int GetId() => SubDepartmentId;
}

internal class GetFinanceSubDepartmentRequestValidator : AbstractPermissionsValidator<GetFinanceSubDepartmentRequest>
{
    public GetFinanceSubDepartmentRequestValidator(IRules<SubDepartment> rules, IUserContext userContext, IStringLocalizer<SharedResource> localizer)
        : base(rules, userContext, localizer)
    {
    }
}