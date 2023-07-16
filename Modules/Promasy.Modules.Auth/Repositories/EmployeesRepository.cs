using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Promasy.Application.Interfaces;
using Promasy.Modules.Auth.Dtos;
using Promasy.Modules.Auth.Helpers;
using Promasy.Modules.Auth.Interfaces;
using Promasy.Modules.Auth.UserContext;

namespace Promasy.Modules.Auth.Repositories;

internal class EmployeesRepository : IEmployeesRepository
{
    private readonly IDatabase _database;

    public EmployeesRepository(IDatabase database)
    {
        _database = database;
    }

    public Task<EmployeePasswordDto?> GetEmployeeDataByUserNameAsync(string userName)
    {
        return _database.Employees
            .AsNoTracking()
            .Where(e => e.UserName == userName.ToLower())
            .Select(e => new EmployeePasswordDto(e.Id, e.Password, e.Salt))
            .FirstOrDefaultAsync();
    }

    public async Task<Claim[]?> GetEmployeeClaimsByIdAsync(int id)
    {
        var employee = await _database.Employees
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new {
                e.Id, 
                e.FirstName, 
                e.MiddleName, 
                e.LastName, 
                e.Email,
                Organization = e.SubDepartment.Department.Organization.Name,
                e.SubDepartment.Department.OrganizationId,
                Department =e.SubDepartment.Department.Name,
                e.SubDepartment.DepartmentId,
                SubDepartment = e.SubDepartment.Name,
                e.SubDepartmentId,
                Roles = e.Roles.Select(r => (int)r.Name).ToArray()
            })
            .FirstOrDefaultAsync();

        if (employee is null)
        {
            return null;
        }

        return new[]
        {
            new Claim(ClaimTypes.Name, employee.Id.ToString()),
            new Claim(ClaimTypes.GivenName, employee.FirstName),
            new Claim(PromasyClaims.MiddleName, employee.MiddleName ?? string.Empty),
            new Claim(ClaimTypes.Surname, employee.LastName),
            new Claim(ClaimTypes.Email, employee.Email),
            new Claim(PromasyClaims.Organization, employee.Organization),
            new Claim(PromasyClaims.OrganizationId, employee.OrganizationId.ToString()),
            new Claim(PromasyClaims.Department, employee.Department),
            new Claim(PromasyClaims.DepartmentId, employee.DepartmentId.ToString()),
            new Claim(PromasyClaims.SubDepartment, employee.SubDepartment),
            new Claim(PromasyClaims.SubDepartmentId, employee.SubDepartmentId.ToString()),
            new Claim(ClaimTypes.Role, string.Join(',', employee.Roles)),
        };
    }
    
    public async Task SetEmployeePasswordAsync(int id, string hash)
    {
        var user = await _database.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (user is null)
        {
            return;
        }

        user.Password = hash;
        user.Salt = null;
        await _database.SaveChangesAsync();
    }
}