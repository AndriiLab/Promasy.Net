using FluentValidation;
using Microsoft.AspNetCore.Http;
using Promasy.Modules.Core.Requests;

namespace Promasy.Modules.Employees.Models;

public class EmployeesPagedRequest : PagedRequest
{
    public int? DepartmentId { get; set; }
    public int? SubDepartmentId { get; set; }

    public EmployeesPagedRequest()
    {
    }

    public EmployeesPagedRequest(int page, int offset, string? search, string? orderBy, bool isDescending,
        int? departmentId, int? subDepartmentId)
        : base(page, offset, search, orderBy, isDescending, null)
    {
        DepartmentId = departmentId;
        SubDepartmentId = subDepartmentId;
    }
    
    public new static ValueTask<EmployeesPagedRequest?> BindAsync(HttpContext httpContext)
    {
        return ValueTask.FromResult<EmployeesPagedRequest?>(new EmployeesPagedRequest(
            int.TryParse(httpContext.Request.Query["page"], out var level) ? level : 1,
            int.TryParse(httpContext.Request.Query["offset"], out var id) ? id : 10,
            httpContext.Request.Query["search"],
            httpContext.Request.Query["order"],
            bool.TryParse(httpContext.Request.Query["desc"], out var desc) && desc,
            int.TryParse(httpContext.Request.Query["department"], out var departmentId) ? departmentId : null,
            int.TryParse(httpContext.Request.Query["sub-department"], out var sudDepartmentId) ? sudDepartmentId : null));
    }
}

internal class EmployeesPagedRequestValidator : AbstractValidator<EmployeesPagedRequest>
{
    public EmployeesPagedRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.Offset)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.Search)
            .MaximumLength(100);
        
        RuleFor(r => r.OrderBy)
            .MaximumLength(100);
        
        RuleFor(r => r.DepartmentId)
            .GreaterThanOrEqualTo(1);
        
        RuleFor(r => r.SubDepartmentId)
            .GreaterThanOrEqualTo(1);
    }
}