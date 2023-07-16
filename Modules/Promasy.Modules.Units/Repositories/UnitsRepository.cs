using Microsoft.EntityFrameworkCore;
using Promasy.Application.Interfaces;
using Promasy.Application.Persistence;
using Promasy.Domain.Employees;
using Promasy.Domain.Orders;
using Promasy.Modules.Core.Pagination;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Units.Dtos;
using Promasy.Modules.Units.Interfaces;
using Z.EntityFramework.Plus;

namespace Promasy.Modules.Units.Repositories;

internal class UnitsRepository : IUnitRules, IUnitsRepository
{
    private readonly IDatabase _database;
    private readonly IUserContext _userContext;

    public UnitsRepository(IDatabase database, IUserContext userContext)
    {
        _database = database;
        _userContext = userContext;
    }

    public Task<bool> IsExistsAsync(int id, CancellationToken ct)
    {
        return _database.Units.AnyAsync(u => u.Id == id, ct);
    }

    public async Task<bool> IsNameUniqueAsync(string name, CancellationToken ct)
    {
        return await _database.Units.AnyAsync(u => EF.Functions.ILike(u.Name, name), ct) == false;
    }

    public async Task<bool> IsNameUniqueAsync(string name, int id, CancellationToken ct)
    {
        return await _database.Units.Where(u => u.Id != id).AnyAsync(u => EF.Functions.ILike(u.Name, name), ct) == false;
    }

    public Task<bool> IsEditableAsync(int id, CancellationToken ct)
    {
        return !_userContext.HasRoles((int)RoleName.User)
            ? Task.FromResult(true)
            : _database.Units.Where(u => u.Id == id).AllAsync(u => u.CreatorId == _userContext.GetId(), ct);
    }

    public Task<bool> IsUsedAsync(int id, CancellationToken ct)
    {
        return _database.Orders.AnyAsync(b => b.UnitId == id, ct);
    }

    public Task<PagedResponse<UnitDto>> GetPagedListAsync(PagedRequest request)
    {
        var query = _database.Units
            .AsNoTracking();
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(u => EF.Functions.ILike(u.Name, $"%{request.Search}%"));
        }

        return query
            .PaginateAsync(request,
                u => new UnitDto(u.Id, u.Name, u.ModifierId ?? u.CreatorId,
                    PromasyDbFunction.GetEmployeeShortName(u.ModifierId ?? u.CreatorId),
                    u.ModifiedDate ?? u.CreatedDate));
    }

    public Task<UnitDto?> GetByIdAsync(int id)
    {
        return _database.Units
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UnitDto(u.Id, u.Name, u.ModifierId ?? u.CreatorId,
                PromasyDbFunction.GetEmployeeShortName(u.ModifierId ?? u.CreatorId), u.ModifiedDate ?? u.CreatedDate))
            .FirstOrDefaultAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var unit = await _database.Units.FirstOrDefaultAsync(u => u.Id == id);
        if (unit is null)
        {
            return;
        }

        _database.Units.Remove(unit);
        await _database.SaveChangesAsync();
    }

    public async Task<int> CreateAsync(UnitDto unit)
    {
        var entity = new Unit {Name = unit.Name};
        _database.Units.Add(entity);
        await _database.SaveChangesAsync();

        return entity.Id;
    }

    public async Task UpdateAsync(UnitDto unit)
    {
        var entity = await _database.Units.FirstAsync(u => u.Id == unit.Id);
        entity.Name = unit.Name;
        await _database.SaveChangesAsync();
    }
    
    public async Task MergeAsync(int targetId, int[] sourceIds)
    {
        await _database.Orders
            .Where(o => sourceIds.Contains(o.UnitId))
            .UpdateAsync(o => new Order {UnitId = targetId});

        var unitsToDelete = await _database.Units
            .Where(m => sourceIds.Contains(m.Id))
            .ToListAsync();
        
        _database.Units.RemoveRange(unitsToDelete);
        await _database.SaveChangesAsync();
    }
}