using FluentValidation;
using Promasy.Modules.Core.Requests;

namespace Promasy.Modules.Orders.Models;

public record OrderSuggestionPagedRequest(int Page = 1, int Offset = 10, string? Search = null, string? OrderBy = null,
        bool IsDescending = false, string? CatNum = null, int? ExcludeId = null)
    : PagedRequest(Page, Offset, Search, OrderBy, IsDescending);

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