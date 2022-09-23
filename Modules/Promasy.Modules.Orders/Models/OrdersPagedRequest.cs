using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Domain.Orders;
using Promasy.Modules.Core.Requests;

namespace Promasy.Modules.Orders.Models;

public class OrdersPagedRequest : PagedRequest
{
    public int? DepartmentId { get; set; }
    public int? SubDepartmentId { get; set; }
    public int? FinanceSourceId { get; set; }
    public OrderType Type { get; set; }

    public OrdersPagedRequest()
    {
    }

    public OrdersPagedRequest(int page, int offset, string? search, string? orderBy, bool isDescending,
        int? departmentId, int? subDepartmentId, int? financeSourceId, OrderType type, int? year)
        : base(page, offset, search, orderBy, isDescending, year)
    {
        DepartmentId = departmentId;
        SubDepartmentId = subDepartmentId;
        FinanceSourceId = financeSourceId;
        Type = type;
        Year = year;
    }
    
    public static ValueTask<OrdersPagedRequest?> BindAsync(HttpContext httpContext)
    {
        return ValueTask.FromResult<OrdersPagedRequest?>(new OrdersPagedRequest(
            int.TryParse(httpContext.Request.Query["page"], out var level) ? level : 1,
            int.TryParse(httpContext.Request.Query["offset"], out var id) ? id : 10,
            httpContext.Request.Query["search"],
            httpContext.Request.Query["order"],
            bool.TryParse(httpContext.Request.Query["desc"], out var desc) && desc,
            int.TryParse(httpContext.Request.Query["department"], out var d) ? d : null,
            int.TryParse(httpContext.Request.Query["subDepartment"], out var sd) ? sd : null,
            int.TryParse(httpContext.Request.Query["finance"], out var fs) ? fs : null,
            (OrderType) (int.TryParse(httpContext.Request.Query["type"], out var t) ? t : 1),
            int.TryParse(httpContext.Request.Query["year"], out var year) ? year : null));
    }
}

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