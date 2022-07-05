using Microsoft.EntityFrameworkCore;
using Promasy.Domain.Persistence;
using Promasy.Modules.Auth.Dtos;
using Promasy.Modules.Auth.Interfaces;

namespace Promasy.Modules.Auth.Repositories;

internal class EmployeesRepository : IEmployeesRepository
{
    private readonly IDatabase _database;

    public EmployeesRepository(IDatabase database)
    {
        _database = database;
    }
    
    public Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
    {
        return _database.Employees
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EmployeeDto(
                e.Id, 
                e.FirstName, 
                e.MiddleName, 
                e.LastName, 
                e.Email,
                e.SubDepartment.Department.Organization.Name,
                e.SubDepartment.Department.OrganizationId,
                e.SubDepartment.Department.Name,
                e.SubDepartment.DepartmentId,
                e.SubDepartment.Name,
                e.SubDepartmentId,
                e.Roles.Select(r => (int)r.Name).ToArray()))
            .FirstOrDefaultAsync();
    }
}