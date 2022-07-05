using Microsoft.EntityFrameworkCore;
using Promasy.Core.UserContext;
using Promasy.Domain.Employees;
using Promasy.Domain.Organizations;
using Promasy.Domain.Persistence;
using Promasy.Modules.Core.Pagination;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Organizations.Dtos;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Repositories;

internal class SubDepartmentsRepository : ISubDepartmentsRules, ISubDepartmentsRepository
{
    private readonly IDatabase _database;
    private readonly IUserContext _userContext;

    public SubDepartmentsRepository(IDatabase database, IUserContext userContext)
    {
        _database = database;
        _userContext = userContext;
    }

    public Task<bool> IsExistAsync(int id, int departmentId, int organizationId, CancellationToken ct)
    {
        return _database.SubDepartments
            .Where(s => s.DepartmentId == departmentId && s.Department.OrganizationId == organizationId)
            .AnyAsync(s => s.Id == id, ct);
    }

    public Task<bool> IsNameUniqueAsync(string name, int departmentId, int organizationId, CancellationToken ct)
    {
        return _database.SubDepartments
            .Where(s => s.DepartmentId == departmentId && s.Department.OrganizationId == organizationId)
            .AllAsync(s => s.Name != name, ct);
    }

    public Task<bool> IsNameUniqueAsync(string name, int id, int departmentId, int organizationId, CancellationToken ct)
    {
        return _database.SubDepartments
            .Where(s => s.DepartmentId == departmentId && s.Department.OrganizationId == organizationId && s.Id != id)
            .AllAsync(s => s.Name != name, ct);
    }

    public bool IsEditable(int id, int departmentId, int organizationId)
    {
        if (_userContext.Roles.Any(r => r is (int) RoleName.Administrator))
        {
            return true;
        }

        if (_userContext.Roles.Any(r => r is (int) RoleName.Director or (int) RoleName.DeputyDirector))
        {
            return _userContext.OrganizationId == organizationId;
        }

        if (_userContext.Roles.Any(r => r is (int) RoleName.HeadOfDepartment))
        {
            return _userContext.DepartmentId == id;
        }

        return false;
    }

    public Task<bool> IsUsedAsync(int id, CancellationToken ct)
    {
        return _database.SubDepartments.AnyAsync(s => s.DepartmentId == id, ct);
    }

    public Task<PagedResponse<SubDepartmentDto>> GetPagedListAsync(int departmentId, PagedRequest request)
    {
        var query = _database.SubDepartments
            .AsNoTracking()
            .Where(d => d.DepartmentId == departmentId);
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(u => u.Name.Contains(request.Search));
        }

        return query
            .PaginateAsync(request,
                s => new SubDepartmentDto(s.Id, s.Name, s.Department.OrganizationId, s.DepartmentId,
                    s.ModifierId ?? s.CreatorId,
                    PromasyDbFunction.GetEmployeeShortName(s.ModifierId ?? s.CreatorId),
                    s.ModifiedDate ?? s.CreatedDate));
    }

    public Task<SubDepartmentDto?> GetByIdAsync(int id)
    {
        return _database.SubDepartments
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SubDepartmentDto(s.Id, s.Name, s.Department.OrganizationId, s.DepartmentId,
                s.ModifierId ?? s.CreatorId,
                PromasyDbFunction.GetEmployeeShortName(s.ModifierId ?? s.CreatorId),
                s.ModifiedDate ?? s.CreatedDate))
            .FirstOrDefaultAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var subDepartment = await _database.SubDepartments.FirstOrDefaultAsync(u => u.Id == id);
        if (subDepartment is null)
        {
            return;
        }

        _database.SubDepartments.Remove(subDepartment);
        await _database.SaveChangesAsync();
    }

    public async Task<int> CreateAsync(SubDepartmentDto item)
    {
        var entity = new SubDepartment
        {
            Name = item.Name,
            DepartmentId = item.DepartmentId
        };
        _database.SubDepartments.Add(entity);
        await _database.SaveChangesAsync();

        return entity.Id;
    }

    public async Task UpdateAsync(SubDepartmentDto item)
    {
        var entity = await _database.SubDepartments
            .FirstAsync(s => s.Id == item.Id);
        entity.Name = item.Name;

        await _database.SaveChangesAsync();
    }
}