using Microsoft.AspNetCore.Mvc;
using Promasy.Modules.Core.Requests;

namespace Promasy.Modules.Finances.Models;

public record GetFinanceSubDepartmentsPagedRequest(
    [FromRoute] int SubDepartmentId,
    [FromRoute] int DepartmentId,
    int Page = 1,
    int Offset = 10,
    string? Search = null,
    [FromQuery(Name = "order")] string? OrderBy = null,
    [FromQuery(Name = "desc")] bool IsDescending = false,
    int? Year = null) : PagedRequest(Page, Offset, Search, OrderBy, IsDescending, Year);