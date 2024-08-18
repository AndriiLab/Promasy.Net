using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Finances.Interfaces;

namespace Promasy.Modules.Finances.Models;

public record GetFinanceSubDepartmentsPagedRequest(
    [FromRoute] int SubDepartmentId,
    [FromRoute] int DepartmentId,
    int Page = 1,
    int Offset = 10,
    string? Search = null,
    [FromQuery(Name = "order")] string? OrderBy = null,
    [FromQuery(Name = "desc")] bool IsDescending = false,
    int? Year = null) : PagedRequest(Page, Offset, Search, OrderBy, IsDescending, Year), IRequestWithPermissionValidation
{
    public int GetId() => DepartmentId;
}

internal class GetFinanceSubDepartmentsPagedRequestValiadtor : AbstractPermissionsValidator<GetFinanceSubDepartmentsPagedRequest>
{
    public GetFinanceSubDepartmentsPagedRequestValiadtor(IFinanceSubDepartmentRules rules, IUserContext userContext, IStringLocalizer<SharedResource> localizer)
        : base(rules, userContext, localizer)
    {
    }
}