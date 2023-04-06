using Microsoft.EntityFrameworkCore;
using Promasy.Application.Interfaces;
using Promasy.Domain.Employees;
using Promasy.Domain.Persistence;
using Promasy.Modules.Core.Pagination;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Employees.Dtos;
using Promasy.Modules.Employees.Interfaces;
using Promasy.Modules.Employees.Models;

namespace Promasy.Modules.Employees.Repositories;

internal class EmployeesRepository : IEmployeeRules, IEmployeesRepository
{
    private readonly IUserContext _userContext;
    private readonly IDatabase _database;

    public EmployeesRepository(IUserContext userContext, IDatabase database)
    {
        _userContext = userContext;
        _database = database;
    }
    
    public Task<bool> IsExistsAsync(int id, CancellationToken ct)
    {
        return _database.Employees.AnyAsync(e => e.Id == id, ct);
    }

    public bool CanChangePasswordForEmployee(int id)
    {
        return _userContext.HasRoles((int) RoleName.Administrator) || _userContext.GetId() == id;
    }

    public bool IsEditable(int id)
    {
        return _userContext.HasRoles((int) RoleName.Administrator) || _userContext.GetId() == id;
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken ct)
    {
        return await _database.Employees
                .AnyAsync(e => EF.Functions.ILike(e.Email, email), ct) == false;;
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int id, CancellationToken ct)
    {
        return await _database.Employees.Where(e => e.Id != id)
            .AnyAsync(e => EF.Functions.ILike(e.Email, email), ct) == false;;
    }

    public Task<bool> IsPhoneUniqueAsync(string phone, CancellationToken ct)
    {
        return _database.Employees.AllAsync(e => e.PrimaryPhone != phone && e.ReservePhone != phone, ct);
    }

    public Task<bool> IsPhoneUniqueAsync(string phone, int id, CancellationToken ct)
    {
        return _database.Employees.Where(e => e.Id != id)
            .AllAsync(e => e.PrimaryPhone != phone && e.ReservePhone != phone, ct);
    }

    public async Task<bool> IsUserNameUniqueAsync(string userName, CancellationToken ct)
    {
        return await _database.Employees
            .AnyAsync(e => EF.Functions.ILike(e.UserName, userName), ct) == false;
    }

    public bool CanHaveRoles(RoleName[] roles)
    {
        return _userContext.HasRoles((int) RoleName.Administrator) || roles.All(r => r == RoleName.User);
    }

    public async Task<bool> CanHaveRolesAsync(RoleName[] roles, int id, CancellationToken ct)
    {
        if (_userContext.HasRoles((int) RoleName.Administrator))
        {
            return true;
        }

        var existingRoles = await _database.Employees
            .AsNoTracking()
            .Where(e => e.Id == id)
            .SelectMany(e => e.Roles)
            .Select(r => r.Name)
            .ToListAsync(ct);

        return !existingRoles.Except(roles).Any() || roles.All(r => r == RoleName.User);
    }

    public Task<PagedResponse<EmployeeShortDto>> GetPagedListAsync(EmployeesPagedRequest request)
    {
        var query = _database.Employees
            .AsNoTracking();

        if (request.SubDepartmentId.HasValue)
        {
            query = query.Where(e => e.SubDepartmentId == request.SubDepartmentId);
        }
        else if(request.DepartmentId.HasValue)
        {
            query = query.Where(e => e.SubDepartment.DepartmentId == request.DepartmentId);
        }

        if (request.Roles?.Any() ?? false)
        {
            query = query.Where(e => e.Roles.Any(r => request.Roles.Contains(r.Name)));
        }
        
        if (!string.IsNullOrEmpty(request.Search))
        {
            var pattern = $"%{request.Search}%";
            query = query.Where(e =>
                EF.Functions.ILike(e.FirstName, pattern) ||
                EF.Functions.ILike(e.LastName, pattern) ||
                EF.Functions.ILike(e.MiddleName, pattern) ||
                EF.Functions.ILike(e.Email, pattern) ||
                e.PrimaryPhone.Contains(request.Search) ||
                e.ReservePhone.Contains(request.Search));
        }

        return query
            .PaginateAsync(request,
                e => new EmployeeShortDto(e.Id, e.ShortName,
                    e.SubDepartment.Department.Name,
                    e.SubDepartment.Name,
                    e.Roles.Select(r => r.Name).ToArray(), e.ModifierId ?? e.CreatorId,
                    PromasyDbFunction.GetEmployeeShortName(e.ModifierId ?? e.CreatorId),
                    e.ModifiedDate ?? e.CreatedDate));
    }

    public Task<EmployeeDto?> GetByIdAsync(int id)
    {
        return _database.Employees
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EmployeeDto(e.Id, e.FirstName, e.MiddleName, e.LastName, e.Email, e.PrimaryPhone,
                e.ReservePhone, e.SubDepartment.DepartmentId, e.SubDepartment.Department.Name, e.SubDepartmentId,
                e.SubDepartment.Name, e.Roles.Select(r => r.Name).ToArray(), e.ModifierId ?? e.CreatorId,
                PromasyDbFunction.GetEmployeeShortName(e.ModifierId ?? e.CreatorId), e.ModifiedDate ?? e.CreatedDate))
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(CreateEmployeeDto employee)
    {
        var roles = await _database.Roles
            .Where(r => employee.Roles.Contains(r.Name))
            .ToListAsync();

        var entity = new Employee
        {
            FirstName = employee.FirstName,
            MiddleName = employee.MiddleName,
            LastName = employee.LastName,
            UserName = employee.UserName.ToLower(),
            Email = employee.Email.ToLower(),
            PrimaryPhone = employee.PrimaryPhone,
            ReservePhone = employee.ReservePhone,
            Password = string.Empty,
            SubDepartmentId = employee.SubDepartmentId,
            Roles = roles
        };

        _database.Employees.Add(entity);
        await _database.SaveChangesAsync();

        return entity.Id;
    }

    public async Task UpdateAsync(UpdateEmployeeDto employee)
    {
        var entity = await _database.Employees
            .Include(e => e.Roles)
            .Where(e => e.Id == employee.Id)
            .FirstOrDefaultAsync();
        if (entity is null)
        {
            return;
        }

        entity.FirstName = employee.FirstName;
        entity.MiddleName = employee.MiddleName;
        entity.LastName = employee.LastName;
        entity.Email = employee.Email.ToLower();
        entity.PrimaryPhone = employee.PrimaryPhone;
        entity.ReservePhone = employee.ReservePhone;
        entity.SubDepartmentId = employee.SubDepartmentId;

        if (entity.Roles.Select(r => r.Name).Except(employee.Roles).Any())
        {
            var roles = await _database.Roles
                .Where(r => employee.Roles.Contains(r.Name))
                .ToListAsync();
            entity.Roles = roles;
        }

        await _database.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var employee = await _database.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (employee is null)
        {
            return;
        }

        _database.Employees.Remove(employee);
        await _database.SaveChangesAsync();
    }
}