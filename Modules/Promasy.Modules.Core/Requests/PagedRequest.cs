using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Promasy.Modules.Core.Requests;

public class PagedRequest
{
    public int Page { get; set; }
    public int Offset { get; set; }
    public string? Search { get; set; }
    public string? OrderBy { get; set; }
    public bool IsDescending { get; set; }
    public int? Year { get; set; }

    public PagedRequest()
    {
    }

    public PagedRequest(int page, int offset, string? search, string? orderBy, bool isDescending, int? year)
    {
        Page = page;
        Offset = offset;
        Search = search;
        OrderBy = orderBy;
        IsDescending = isDescending;
        Year = year;
    }

    public static ValueTask<PagedRequest?> BindAsync(HttpContext httpContext)
    {
        return ValueTask.FromResult<PagedRequest?>(new PagedRequest(
            int.TryParse(httpContext.Request.Query["page"], out var level) ? level : 1,
            int.TryParse(httpContext.Request.Query["offset"], out var id) ? id : 10,
            httpContext.Request.Query["search"],
            httpContext.Request.Query["order"],
            bool.TryParse(httpContext.Request.Query["desc"], out var desc) && desc,
            int.TryParse(httpContext.Request.Query["year"], out var y) ? y : null));
    }
}

public class PagedRequestValidator : AbstractValidator<PagedRequest>
{
    public PagedRequestValidator()
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
            .GreaterThan(2000);
    }
}