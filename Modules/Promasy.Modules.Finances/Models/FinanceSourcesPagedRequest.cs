using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Promasy.Modules.Core.Requests;

namespace Promasy.Modules.Finances.Models;

public record FinanceSourcesPagedRequest(
        int Page = 1,
        int Offset = 10,
        string? Search = null,
        [FromQuery(Name = "order")] string? OrderBy = null,
        [FromQuery(Name = "desc")] bool IsDescending = false,
        int? Year = null,
        [FromQuery(Name = "extended")] bool IncludeCalculatedAmounts = false)
    : PagedRequest(Page, Offset, Search, OrderBy, IsDescending, Year);

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
            .GreaterThan(2000);
    }
}