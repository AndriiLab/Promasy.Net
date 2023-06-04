using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Domain.Orders;
using Promasy.Modules.Core.Requests;

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
    : PagedRequest(Page, Offset, Search, OrderBy, IsDescending, Year);

public class OrdersPagedRequestValidator : AbstractValidator<OrdersPagedRequest>
{
    public OrdersPagedRequestValidator(IStringLocalizer<SharedResource> localizer)
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
    }
}