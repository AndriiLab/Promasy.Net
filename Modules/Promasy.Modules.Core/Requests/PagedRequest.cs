using FluentValidation;

namespace Promasy.Modules.Core.Requests;

public record PagedRequest(int Page = 1, int Offset = 10, string? Search = null, string? OrderBy = null,
    bool IsDescending = false, int? Year = null);

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