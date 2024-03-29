﻿using Microsoft.EntityFrameworkCore;
using Promasy.Application.Interfaces;
using Promasy.Application.Persistence;
using Promasy.Domain.Employees;
using Promasy.Domain.Organizations;
using Promasy.Modules.Core.Pagination;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Organizations.Dtos;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Repositories;

internal class DepartmentsRepository : IDepartmentRules, IDepartmentsRepository
{
    private readonly IDatabase _database;
    private readonly IUserContext _userContext;

    public DepartmentsRepository(IDatabase database, IUserContext userContext)
    {
        _database = database;
        _userContext = userContext;
    }

    public Task<bool> IsExistsAsync(int id, CancellationToken ct)
    {
        return _database.Departments.AnyAsync(d => d.Id == id, ct);
    }

    public async Task<bool> IsNameUniqueAsync(string name, CancellationToken ct)
    {
        return await _database.Departments.AnyAsync(d => EF.Functions.ILike(d.Name, name), ct) == false;
    }

    public async Task<bool> IsNameUniqueAsync(string name, int id, CancellationToken ct)
    {
        return await _database.Departments.Where(u => u.Id != id).AnyAsync(d => EF.Functions.ILike(d.Name, name), ct) == false;
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

    public Task<PagedResponse<DepartmentDto>> GetPagedListAsync(PagedRequest request)
    {
        var query = _database.Departments
            .AsNoTracking();
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(d => EF.Functions.ILike(d.Name, $"%{request.Search}%"));
        }

        return query
            .PaginateAsync(request,
                d => new DepartmentDto(d.Id, d.Name, d.OrganizationId, d.ModifierId ?? d.CreatorId,
                    PromasyDbFunction.GetEmployeeShortName(d.ModifierId ?? d.CreatorId),
                    d.ModifiedDate ?? d.CreatedDate));
    }

    public Task<DepartmentDto?> GetByIdAsync(int id)
    {
        return _database.Departments
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(d => new DepartmentDto(d.Id, d.Name, d.OrganizationId, d.ModifierId ?? d.CreatorId,
                    PromasyDbFunction.GetEmployeeShortName(d.ModifierId ?? d.CreatorId),
                    d.ModifiedDate ?? d.CreatedDate))
            .FirstOrDefaultAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var department = await _database.Departments.FirstOrDefaultAsync(u => u.Id == id);
        if (department is null)
        {
            return;
        }

        _database.Departments.Remove(department);
        await _database.SaveChangesAsync();
    }

    public async Task<int> CreateAsync(DepartmentDto item)
    {
        var entity = new Department
        {
            Name = item.Name,
            OrganizationId = item.OrganizationId,
            SubDepartments = new List<SubDepartment>
            {
                new() { Name = SubDepartmentName.Default }
            }
        };
        _database.Departments.Add(entity);
        await _database.SaveChangesAsync();

        return entity.Id;
    }

    public async Task UpdateAsync(DepartmentDto item)
    {
        var entity = await _database.Departments
            .FirstAsync(o => o.Id == item.Id);
        entity.Name = item.Name;

        await _database.SaveChangesAsync();
    }
}