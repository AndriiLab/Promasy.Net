using FluentValidation;
using Microsoft.AspNetCore.Http;
using Promasy.Core.Persistence;

namespace Promasy.Modules.Cpv.Models;

public class GetCpvsRequest
{
    public int? Level { get; set; }
    public int? ParentId { get; set; }
    public string? Search { get; set; }

    public GetCpvsRequest()
    {
    }

    public GetCpvsRequest(int? level, int? parentId, string? search)
    {
        Level = level;
        ParentId = parentId;
        Search = search;
    }

    public static ValueTask<GetCpvsRequest?> BindAsync(HttpContext httpContext)
    {
        var result = new GetCpvsRequest(
            int.TryParse(httpContext.Request.Query["level"], out var level) ? level : null,
            int.TryParse(httpContext.Request.Query["parentId"], out var parentId) ? parentId : null,
            httpContext.Request.Query["search"]);

        if (result.Level is null && result.ParentId is null && string.IsNullOrEmpty(result.Search))
        {
            result.Level = 1;
        }

        return ValueTask.FromResult<GetCpvsRequest?>(result);
    }
}

internal class GetCpvRequestValidator : AbstractValidator<GetCpvsRequest>
{
    public GetCpvRequestValidator()
    {
        RuleFor(r => r.Search)
            .MaximumLength(PersistenceConstant.FieldMedium);
        
        RuleFor(r => r.Level)
            .GreaterThan(0);

        RuleFor(r => r.ParentId)
            .GreaterThan(0);
    }
}