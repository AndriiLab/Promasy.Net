using FluentValidation;
using Microsoft.AspNetCore.Http;
using Promasy.Modules.Core.Requests;

namespace Promasy.Modules.Finances.Models;

public class FinanceSourcesPagedRequest : PagedRequest
{
    public int Year { get; set; }

    public FinanceSourcesPagedRequest(int page, int offset, string? search, string? orderBy, bool isDescending, int year)
        : base(page, offset, search, orderBy, isDescending)
    {
        Year = year;
    }
    
    public static ValueTask<FinanceSourcesPagedRequest?> BindAsync(HttpContext httpContext)
    {
        return ValueTask.FromResult<FinanceSourcesPagedRequest?>(new FinanceSourcesPagedRequest(
            int.TryParse(httpContext.Request.Query["page"], out var level) ? level : 1,
            int.TryParse(httpContext.Request.Query["offset"], out var id) ? id : 10,
            httpContext.Request.Query["search"],
            httpContext.Request.Query["order"],
            bool.TryParse(httpContext.Request.Query["desc"], out var desc) && desc,
            int.TryParse(httpContext.Request.Query["year"], out var y) ? y : DateTime.UtcNow.Year));
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