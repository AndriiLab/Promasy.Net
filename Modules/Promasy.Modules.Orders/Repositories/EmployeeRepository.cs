using Microsoft.EntityFrameworkCore;
using Promasy.Domain.Employees;
using Promasy.Domain.Persistence;
using Promasy.Modules.Orders.Dtos;
using Promasy.Modules.Orders.Interfaces;

namespace Promasy.Modules.Orders.Repositories;

internal class EmployeeRepository : IEmployeeRules, IEmployeesRepository
{
    private readonly IDatabase _database;

    public EmployeeRepository(IDatabase database)
    {
        _database = database;
    }

    public Task<bool> IsExistsWithRoleAsync(int id, RoleName role, CancellationToken ct)
    {
        return _database.Employees.AnyAsync(e => e.Id == id && e.Roles.Any(r => r.Name == role), ct);
    }

    public async Task<List<EmployeeDto>> GetByIdsAndRolesAsync(IEnumerable<Tuple<int, RoleName>> idRoles)
    {
        var idRolesList = idRoles.ToList();
        var employees = await _database.Employees
            .AsNoTracking()
            .Where(e => idRolesList.Select(t => t.Item1).Contains(e.Id))
            .Select(e => new EmployeeDto(e.Id, e.ShortName, e.PrimaryPhone, e.SubDepartment.DepartmentId))
            .ToListAsync();

        foreach (var employee in employees)
        {
            employee.Role = idRolesList.First(r => r.Item1 == employee.Id).Item2;
        }

        return employees;
    }

    public Task<List<EmployeeDto>> GetEmployeesForDepartmentIdAsync(int departmentId, params RoleName[] roles)
    {
        return _database.Employees
            .AsNoTracking()
            .Where(e => e.SubDepartment.DepartmentId == departmentId && e.Roles.Any(r => roles.Contains(r.Name)))
            .Select(e => new EmployeeDto(e.Id, e.ShortName, e.PrimaryPhone, e.SubDepartment.DepartmentId, e.Roles.Where(r => roles.Contains(r.Name)).Select(r => r.Name).First()))
            .ToListAsync();
    }
}