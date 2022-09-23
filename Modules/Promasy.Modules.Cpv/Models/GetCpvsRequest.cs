using FluentValidation;
using Microsoft.AspNetCore.Http;
using Promasy.Core.Persistence;

namespace Promasy.Modules.Cpv.Models;

public class GetCpvsRequest
{
    public int? ParentId { get; set; }
    public int? Id { get; set; }
    public string? Search { get; set; }

    public GetCpvsRequest()
    {
    }

    public GetCpvsRequest(int? parentId, int? id, string? search)
    {
        ParentId = parentId;
        Id = id;
        Search = search;
    }

    public static ValueTask<GetCpvsRequest?> BindAsync(HttpContext httpContext)
    {
        var result = new GetCpvsRequest(
            int.TryParse(httpContext.Request.Query["parentId"], out var parentId) ? parentId : null,
            int.TryParse(httpContext.Request.Query["id"], out var id) ? id : null,
            httpContext.Request.Query["search"]);

        return ValueTask.FromResult<GetCpvsRequest?>(result);
    }
}

internal class GetCpvRequestValidator : AbstractValidator<GetCpvsRequest>
{
    public GetCpvRequestValidator()
    {
        RuleFor(r => r.Search)
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.ParentId)
            .GreaterThan(0);
    }
}