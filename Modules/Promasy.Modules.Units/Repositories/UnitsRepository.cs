using Microsoft.EntityFrameworkCore;
using Promasy.Domain.Orders;
using Promasy.Domain.Persistence;
using Promasy.Modules.Core.Pagination;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Units.Dtos;
using Promasy.Modules.Units.Interfaces;

namespace Promasy.Modules.Units.Repositories;

internal class UnitsRepository : IUnitsRules, IUnitsRepository
{
    private readonly IDatabase _database;

    public UnitsRepository(IDatabase database)
    {
        _database = database;
    }

    public Task<bool> IsExistAsync(int id, CancellationToken ct)
    {
        return _database.Units.AnyAsync(u => u.Id == id, ct);
    }

    public Task<bool> IsNameUniqueAsync(string name, CancellationToken ct)
    {
        return _database.Units.AllAsync(u => u.Name != name, ct);
    }

    public Task<bool> IsNameUniqueAsync(string name, int id, CancellationToken ct)
    {
        return _database.Units.Where(u => u.Id != id).AllAsync(u => u.Name != name, ct);
    }

    public Task<bool> IsUnitUsedAsync(int id, CancellationToken ct)
    {
        return _database.Orders.AnyAsync(b => b.UnitId == id, ct);
    }

    public Task<PagedResponse<UnitDto>> GetUnitsAsync(PagedRequest request)
    {
        var query = _database.Units
            .AsNoTracking();
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(u => u.Name.Contains(request.Search));
        }

        return query
            .PaginateAsync(request, u => new UnitDto(u.Id, u.Name));
    }

    public Task<UnitDto?> GetUnitByIdAsync(int id)
    {
        return _database.Units
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UnitDto(u.Id, u.Name))
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

    public async Task<int> CreateUnitAsync(UnitDto unit)
    {
        var entity = new Unit {Name = unit.Name};
        _database.Units.Add(entity);
        await _database.SaveChangesAsync();

        return entity.Id;
    }

    public async Task UpdateUnitAsync(UnitDto unit)
    {
        var entity = await _database.Units.FirstAsync(u => u.Id == unit.Id);
        entity.Name = unit.Name;
        await _database.SaveChangesAsync();
    }
}