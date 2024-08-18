using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Domain.Organizations;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Validation;

namespace Promasy.Modules.Finances.Models;

public record FinanceSourcesPagedRequest(
    int Page = 1,
    int Offset = 10,
    string? Search = null,
    [FromQuery(Name = "order")] string? OrderBy = null,
    [FromQuery(Name = "desc")] bool IsDescending = false,
    int? Year = null,
    [FromQuery(Name = "extended")] bool IncludeCalculatedAmounts = false)
    : PagedRequest(Page, Offset, Search, OrderBy, IsDescending, Year), IRequestWithPermissionValidation
{
    public int GetId() => throw new NotSupportedException();
}

public class FinanceSourcesPagedRequestValidator : AbstractPermissionsValidator<FinanceSourcesPagedRequest>
{
    public FinanceSourcesPagedRequestValidator(IRules<Department> rules, IUserContext userContext,
        IStringLocalizer<SharedResource> localizer) : base(rules, userContext, localizer)
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