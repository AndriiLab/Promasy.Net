using Microsoft.EntityFrameworkCore;
using Promasy.Application.Interfaces;
using Promasy.Application.Persistence;
using Promasy.Domain.Employees;
using Promasy.Domain.Manufacturers;
using Promasy.Domain.Orders;
using Promasy.Modules.Core.Pagination;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Manufacturers.Dtos;
using Promasy.Modules.Manufacturers.Interfaces;
using Z.EntityFramework.Plus;

namespace Promasy.Modules.Manufacturers.Repositories;

internal class ManufacturersRepository : IManufacturerRules, IManufacturersRepository
{
    private readonly IDatabase _database;
    private readonly IUserContext _userContext;

    public ManufacturersRepository(IDatabase database, IUserContext userContext)
    {
        _database = database;
        _userContext = userContext;
    }

    public Task<bool> IsExistsAsync(int id, CancellationToken ct)
    {
        return _database.Manufacturers.AnyAsync(u => u.Id == id, ct);
    }

    public async Task<bool> IsNameUniqueAsync(string name, CancellationToken ct)
    {
        return await _database.Manufacturers.AnyAsync(m => EF.Functions.ILike(m.Name, name), ct) == false;
    }

    public async Task<bool> IsNameUniqueAsync(string name, int id, CancellationToken ct)
    {
        return await _database.Manufacturers.Where(u => u.Id != id)
            .AnyAsync(m => EF.Functions.ILike(m.Name, name), ct) == false;
    }

    public Task<bool> IsEditableAsync(int id, CancellationToken ct)
    {
        return !_userContext.HasRoles((int)RoleName.User)
            ? Task.FromResult(true)
            : _database.Manufacturers.Where(u => u.Id == id).AllAsync(u => u.CreatorId == _userContext.GetId(), ct);
    }

    public Task<bool> IsUsedAsync(int id, CancellationToken ct)
    {
        return _database.Orders.AnyAsync(b => b.ManufacturerId == id, ct);
    }

    public Task<PagedResponse<ManufacturerDto>> GetPagedListAsync(PagedRequest request)
    {
        var query = _database.Manufacturers
            .AsNoTracking();
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(u => EF.Functions.ILike(u.Name, $"%{request.Search}%"));
        }

        return query
            .PaginateAsync(request,
                u => new ManufacturerDto(u.Id, u.Name, u.ModifierId ?? u.CreatorId,
                    PromasyDbFunction.GetEmployeeShortName(u.ModifierId ?? u.CreatorId),
                    u.ModifiedDate ?? u.CreatedDate));
    }

    public Task<ManufacturerDto?> GetByIdAsync(int id)
    {
        return _database.Manufacturers
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new ManufacturerDto(u.Id, u.Name, u.ModifierId ?? u.CreatorId,
                PromasyDbFunction.GetEmployeeShortName(u.ModifierId ?? u.CreatorId), u.ModifiedDate ?? u.CreatedDate))
            .FirstOrDefaultAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var manufacturer = await _database.Manufacturers.FirstOrDefaultAsync(u => u.Id == id);
        if (manufacturer is null)
        {
            return;
        }

        _database.Manufacturers.Remove(manufacturer);
        await _database.SaveChangesAsync();
    }

    public async Task<int> CreateAsync(ManufacturerDto manufacturer)
    {
        var entity = new Manufacturer {Name = manufacturer.Name};
        _database.Manufacturers.Add(entity);
        await _database.SaveChangesAsync();

        return entity.Id;
    }

    public async Task UpdateAsync(ManufacturerDto manufacturer)
    {
        var entity = await _database.Manufacturers.FirstAsync(u => u.Id == manufacturer.Id);
        entity.Name = manufacturer.Name;
        await _database.SaveChangesAsync();
    }

    public async Task MergeAsync(int targetId, int[] sourceIds)
    {
        await _database.Orders
            .Where(o => o.ManufacturerId.HasValue && sourceIds.Contains(o.ManufacturerId.Value))
            .UpdateAsync(o => new Order {ManufacturerId = targetId});


        var manufacturersToDelete = await _database.Manufacturers
            .Where(m => sourceIds.Contains(m.Id))
            .ToListAsync();
        
        _database.Manufacturers.RemoveRange(manufacturersToDelete);
        await _database.SaveChangesAsync();
    }
}