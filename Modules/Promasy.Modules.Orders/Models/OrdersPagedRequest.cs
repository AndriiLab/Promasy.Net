using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Domain.Employees;
using Promasy.Domain.Orders;
using Promasy.Domain.Organizations;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Validation;

namespace Promasy.Modules.Orders.Models;

public record OrdersPagedRequest(
        int Page = 1,
        int Offset = 10,
        string? Search = null,
        [FromQuery(Name = "order")] string? OrderBy = null,
        [FromQuery(Name = "desc")] bool IsDescending = false,
        int? Year = null,
        [FromQuery(Name = "department")] int? DepartmentId = null,
        [FromQuery(Name = "subDepartment")] int? SubDepartmentId = null,
        [FromQuery(Name = "finance")] int? FinanceSourceId = null,
        [FromQuery(Name = "type")] OrderType Type = OrderType.Equipment)
    : PagedRequest(Page, Offset, Search, OrderBy, IsDescending, Year), IRequestWithPermissionValidation
{
    public int GetId() => DepartmentId ?? 0;
}

public class OrdersPagedRequestValidator : AbstractPermissionsValidator<OrdersPagedRequest>
{
    public OrdersPagedRequestValidator(IStringLocalizer<SharedResource> localizer, IUserContext userContext, IRules<Department> departmentRules)
        : base(departmentRules, userContext, localizer)
    {
        RuleFor(r => r.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.Offset)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.Search)
            .MaximumLength(100);

        RuleFor(r => r.OrderBy)
            .MaximumLength(100);

        RuleFor(r => r.Type)
            .Must(t => Enum.GetValues<OrderType>().Any(ot => ot == t))
            .WithMessage(localizer["Order type not exists"]);

        When(
            _ => userContext.HasRoles((int)RoleName.User, (int)RoleName.HeadOfDepartment,
                (int)RoleName.PersonallyLiableEmployee),
            () =>
            {
                RuleFor(o => o.DepartmentId)
                    .NotEmpty()
                    .GreaterThanOrEqualTo(1);
            });
    }
}