using FluentValidation;
using Microsoft.AspNetCore.Http;
using Promasy.Modules.Core.Requests;

namespace Promasy.Modules.Orders.Models;

public class OrderSuggestionPagedRequest : PagedRequest
{
    public string? CatNum { get; set; }
    public int? ExcludeId { get; set; }

    public OrderSuggestionPagedRequest()
    {
    }

    public OrderSuggestionPagedRequest(int page, int offset, string? search, string? orderBy, bool isDescending, string? catNum, int? excludeId) 
        : base(page, offset, search, orderBy, isDescending, null)
    {
        CatNum = catNum;
        ExcludeId = excludeId;
    }
    
    public static ValueTask<OrderSuggestionPagedRequest?> BindAsync(HttpContext httpContext)
    {
        return ValueTask.FromResult<OrderSuggestionPagedRequest?>(new OrderSuggestionPagedRequest(
            int.TryParse(httpContext.Request.Query["page"], out var level) ? level : 1,
            int.TryParse(httpContext.Request.Query["offset"], out var offset) ? offset : 10,
            httpContext.Request.Query["search"],
            httpContext.Request.Query["order"],
            bool.TryParse(httpContext.Request.Query["desc"], out var desc) && desc,
            httpContext.Request.Query["cat"],
            int.TryParse(httpContext.Request.Query["exclude"], out var id) ? id : null));
    }
}

public class OrderSuggestionPagedRequestValidator : AbstractValidator<OrderSuggestionPagedRequest>
{
    public OrderSuggestionPagedRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.Offset)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.Search)
            .MaximumLength(100);
        
        RuleFor(r => r.OrderBy)
            .MaximumLength(100);

        RuleFor(r => r.CatNum)
            .MaximumLength(100);
        
        RuleFor(r => r.ExcludeId)
            .GreaterThanOrEqualTo(1);

        When(r => string.IsNullOrEmpty(r.CatNum), () =>
        {
            RuleFor(r => r.Search)
                .NotEmpty();
        });
        
        When(r => string.IsNullOrEmpty(r.Search), () =>
        {
            RuleFor(r => r.CatNum)
                .NotEmpty();
        });
    }
}