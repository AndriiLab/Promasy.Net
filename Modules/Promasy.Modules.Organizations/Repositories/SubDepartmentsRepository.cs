using Microsoft.EntityFrameworkCore;
using Promasy.Application.Interfaces;
using Promasy.Domain.Employees;
using Promasy.Domain.Organizations;
using Promasy.Domain.Persistence;
using Promasy.Modules.Core.Pagination;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Organizations.Dtos;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Repositories;

internal class SubDepartmentsRepository : ISubDepartmentRules, ISubDepartmentsRepository
{
    private readonly IDatabase _database;
    private readonly IUserContext _userContext;

    public SubDepartmentsRepository(IDatabase database, IUserContext userContext)
    {
        _database = database;
        _userContext = userContext;
    }

    public Task<bool> IsExistsAsync(int id, CancellationToken ct)
    {
        return _database.SubDepartments.AnyAsync(s => s.Id == id, ct);
    }

    public async Task<bool> IsNameUniqueAsync(string name, int departmentId, CancellationToken ct)
    {
        return await _database.SubDepartments
            .Where(s => s.DepartmentId == departmentId)
            .AnyAsync(d => EF.Functions.ILike(d.Name, name), ct) == false;
    }

    public async Task<bool> IsNameUniqueAsync(string name, int id, int departmentId, CancellationToken ct)
    {
        return await _database.SubDepartments
            .Where(s => s.DepartmentId == departmentId && s.Id != id)
            .AnyAsync(d => EF.Functions.ILike(d.Name, name), ct) == false;
    }

    public bool IsEditable(int id)
    {
        if (_userContext.HasRoles((int) RoleName.Administrator, (int) RoleName.Director, (int) RoleName.DeputyDirector))
        {
            return true;
        }

        if (_userContext.HasRoles((int) RoleName.HeadOfDepartment))
        {
            return _userContext.GetDepartmentId() == id;
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
            query = query.Where(u => EF.Functions.ILike(u.Name, $"%{request.Search}%"));
        }

        return query
            .PaginateAsync(request,
                s => new SubDepartmentDto(s.Id, s.Name, s.DepartmentId,
                    s.ModifierId ?? s.CreatorId,
                    PromasyDbFunction.GetEmployeeShortName(s.ModifierId ?? s.CreatorId),
                    s.ModifiedDate ?? s.CreatedDate));
    }

    public Task<SubDepartmentDto?> GetByIdAsync(int id)
    {
        return _database.SubDepartments
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SubDepartmentDto(s.Id, s.Name, s.DepartmentId,
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