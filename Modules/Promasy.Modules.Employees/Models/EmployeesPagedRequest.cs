using FluentValidation;
using Promasy.Domain.Employees;
using Promasy.Modules.Core.Requests;

namespace Promasy.Modules.Employees.Models;

public record EmployeesPagedRequest(int Page = 1, int Offset = 10, string? Search = null, string? OrderBy = null,
        bool IsDescending = false, int? DepartmentId = null, int? SubDepartmentId = null, RoleName[]? Roles = null)
    : PagedRequest(Page, Offset, Search, OrderBy, IsDescending);

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