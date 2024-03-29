﻿using Microsoft.EntityFrameworkCore;
using Promasy.Application.Interfaces;
using Promasy.Application.Persistence;
using Promasy.Domain.Employees;
using Promasy.Domain.Orders;
using Promasy.Domain.Suppliers;
using Promasy.Modules.Core.Pagination;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Suppliers.Dtos;
using Promasy.Modules.Suppliers.Interfaces;
using Z.EntityFramework.Plus;

namespace Promasy.Modules.Suppliers.Repositories;

internal class SuppliersRepository : ISupplierRules, ISuppliersRepository
{
    private readonly IDatabase _database;
    private readonly IUserContext _userContext;

    public SuppliersRepository(IDatabase database, IUserContext userContext)
    {
        _database = database;
        _userContext = userContext;
    }

    public Task<bool> IsExistsAsync(int id, CancellationToken ct)
    {
        return _database.Suppliers.AnyAsync(u => u.Id == id, ct);
    }

    public async Task<bool> IsNameUniqueAsync(string name, CancellationToken ct)
    {
        return await _database.Suppliers.AnyAsync(u => EF.Functions.ILike(u.Name, name), ct) == false;
    }

    public async Task<bool> IsNameUniqueAsync(string name, int id, CancellationToken ct)
    {
        return await _database.Suppliers.Where(u => u.Id != id).AnyAsync(u => EF.Functions.ILike(u.Name, name), ct) == false;
    }

    public Task<bool> IsEditableAsync(int id, CancellationToken ct)
    {
        return !_userContext.HasRoles((int)RoleName.User)
            ? Task.FromResult(true)
            : _database.Suppliers.Where(u => u.Id == id).AllAsync(u => u.CreatorId == _userContext.GetId(), ct);
    }

    public Task<bool> IsUsedAsync(int id, CancellationToken ct)
    {
        return _database.Orders.AnyAsync(b => b.SupplierId == id, ct);
    }

    public Task<PagedResponse<SupplierDto>> GetPagedListAsync(PagedRequest request)
    {
        var query = _database.Suppliers
            .AsNoTracking();
        if (!string.IsNullOrEmpty(request.Search))
        {
            var pattern = $"%{request.Search}%";
            query = query.Where(u => EF.Functions.ILike(u.Name, pattern) || 
                                     EF.Functions.ILike(u.Comment, pattern) || 
                                     u.Phone.Contains(request.Search));
        }

        return query
            .PaginateAsync(request,
                u => new SupplierDto(u.Id, u.Name, u.Comment, u.Phone, u.ModifierId ?? u.CreatorId,
                    PromasyDbFunction.GetEmployeeShortName(u.ModifierId ?? u.CreatorId),
                    u.ModifiedDate ?? u.CreatedDate));
    }

    public Task<SupplierDto?> GetByIdAsync(int id)
    {
        return _database.Suppliers
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new SupplierDto(u.Id, u.Name, u.Comment, u.Phone, u.ModifierId ?? u.CreatorId,
                PromasyDbFunction.GetEmployeeShortName(u.ModifierId ?? u.CreatorId), u.ModifiedDate ?? u.CreatedDate))
            .FirstOrDefaultAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var unit = await _database.Suppliers.FirstOrDefaultAsync(u => u.Id == id);
        if (unit is null)
        {
            return;
        }

        _database.Suppliers.Remove(unit);
        await _database.SaveChangesAsync();
    }

    public async Task<int> CreateAsync(SupplierDto supplier)
    {
        var entity = new Supplier {Name = supplier.Name, Comment = supplier.Comment, Phone = supplier.Phone};
        _database.Suppliers.Add(entity);
        await _database.SaveChangesAsync();

        return entity.Id;
    }

    public async Task UpdateAsync(SupplierDto supplier)
    {
        var entity = await _database.Suppliers.FirstAsync(u => u.Id == supplier.Id);
        entity.Name = supplier.Name;
        entity.Comment = supplier.Comment;
        entity.Phone = supplier.Phone;
        await _database.SaveChangesAsync();
    }
    
    public async Task MergeAsync(int targetId, int[] sourceIds)
    {
        await _database.Orders
            .Where(o => o.SupplierId.HasValue && sourceIds.Contains(o.SupplierId.Value))
            .UpdateAsync(o => new Order {SupplierId = targetId});

        var unitsToDelete = await _database.Suppliers
            .Where(m => sourceIds.Contains(m.Id))
            .ToListAsync();
        
        _database.Suppliers.RemoveRange(unitsToDelete);
        await _database.SaveChangesAsync();
    }
}