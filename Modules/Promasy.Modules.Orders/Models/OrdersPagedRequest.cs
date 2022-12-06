using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Domain.Orders;
using Promasy.Modules.Core.Requests;

namespace Promasy.Modules.Orders.Models;

public record OrdersPagedRequest(int Page = 1, int Offset = 10, string? Search = null, string? OrderBy = null,
        bool IsDescending = false, int? Year = null,
        int? DepartmentId = null, int? SubDepartmentId = null, int? FinanceSourceId = null,
        OrderType Type = OrderType.Equipment)
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