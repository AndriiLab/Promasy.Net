using FluentValidation;
using Microsoft.AspNetCore.Http;
using Promasy.Modules.Core.Requests;

namespace Promasy.Modules.Finances.Models;

public class FinanceSourcesPagedRequest : PagedRequest
{
    public int Year { get; set; }
    public bool IncludeCalculatedAmounts { get; set; }

    public FinanceSourcesPagedRequest(int page, int offset, string? search, string? orderBy, bool isDescending, int year, bool includeCalculatedAmounts)
        : base(page, offset, search, orderBy, isDescending)
    {
        Year = year;
        IncludeCalculatedAmounts = includeCalculatedAmounts;
    }
    
    public static ValueTask<FinanceSourcesPagedRequest?> BindAsync(HttpContext httpContext)
    {
        return ValueTask.FromResult<FinanceSourcesPagedRequest?>(new FinanceSourcesPagedRequest(
            int.TryParse(httpContext.Request.Query["page"], out var level) ? level : 1,
            int.TryParse(httpContext.Request.Query["offset"], out var id) ? id : 10,
            httpContext.Request.Query["search"],
            httpContext.Request.Query["order"],
            bool.TryParse(httpContext.Request.Query["desc"], out var desc) && desc,
            int.TryParse(httpContext.Request.Query["year"], out var y) ? y : DateTime.UtcNow.Year,
            bool.TryParse(httpContext.Request.Query["extended"], out var ext) && ext));
    }
}

public class FinanceSourcesPagedRequestValidator : AbstractValidator<FinanceSourcesPagedRequest>
{
    public FinanceSourcesPagedRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.Offset)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.Search)
            .MaximumLength(100);
        
        RuleFor(r => r.OrderBy)
            .MaximumLength(100);
        
        RuleFor(r => r.Year)
            .GreaterThan(1);
    }
}