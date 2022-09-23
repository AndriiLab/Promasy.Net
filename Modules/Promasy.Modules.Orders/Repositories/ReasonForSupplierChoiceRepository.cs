using Microsoft.EntityFrameworkCore;
using Promasy.Core.UserContext;
using Promasy.Domain.Employees;
using Promasy.Domain.Orders;
using Promasy.Domain.Persistence;
using Promasy.Modules.Core.Pagination;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Orders.Dtos;
using Promasy.Modules.Orders.Interfaces;
using Z.EntityFramework.Plus;

namespace Promasy.Modules.Orders.Repositories;

internal class ReasonForSupplierChoiceRepository : IReasonForSupplierChoiceRules, IReasonForSupplierChoiceRepository
{
    private readonly IDatabase _database;
    private readonly IUserContext _userContext;

    public ReasonForSupplierChoiceRepository(IDatabase database, IUserContext userContext)
    {
        _database = database;
        _userContext = userContext;
    }

    public Task<bool> IsExistsAsync(int id, CancellationToken ct)
    {
        return _database.ReasonForSupplierChoice.AnyAsync(r => r.Id == id, ct);
    }

    public async Task<bool> IsNameUniqueAsync(string name, CancellationToken ct)
    {
        return await _database.ReasonForSupplierChoice.AnyAsync(r => EF.Functions.ILike(r.Name, name), ct) == false;
    }

    public async Task<bool> IsNameUniqueAsync(string name, int id, CancellationToken ct)
    {
        return await _database.ReasonForSupplierChoice.Where(r => r.Id != id).AnyAsync(u => EF.Functions.ILike(u.Name, name), ct) == false;
    }

    public Task<bool> IsEditableAsync(int id, CancellationToken ct)
    {
        return !_userContext.HasRoles((int)RoleName.User)
            ? Task.FromResult(true)
            : _database.ReasonForSupplierChoice.Where(r => r.Id == id).AllAsync(r => r.CreatorId == _userContext.GetId(), ct);
    }

    public Task<bool> IsUsedAsync(int id, CancellationToken ct)
    {
        return _database.Orders.AnyAsync(o => o.ReasonId == id, ct);
    }

    public Task<PagedResponse<ReasonForSupplierChoiceDto>> GetPagedListAsync(PagedRequest request)
    {
        var query = _database.ReasonForSupplierChoice
            .AsNoTracking();
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(r => EF.Functions.ILike(r.Name, $"%{request.Search}%"));
        }

        return query
            .PaginateAsync(request,
                r => new ReasonForSupplierChoiceDto(r.Id, r.Name, r.ModifierId ?? r.CreatorId,
                    PromasyDbFunction.GetEmployeeShortName(r.ModifierId ?? r.CreatorId),
                    r.ModifiedDate ?? r.CreatedDate));
    }

    public Task<ReasonForSupplierChoiceDto?> GetByIdAsync(int id)
    {
        return _database.ReasonForSupplierChoice
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new ReasonForSupplierChoiceDto(r.Id, r.Name, r.ModifierId ?? r.CreatorId,
                PromasyDbFunction.GetEmployeeShortName(r.ModifierId ?? r.CreatorId), r.ModifiedDate ?? r.CreatedDate))
            .FirstOrDefaultAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var reason = await _database.ReasonForSupplierChoice.FirstOrDefaultAsync(u => u.Id == id);
        if (reason is null)
        {
            return;
        }

        _database.ReasonForSupplierChoice.Remove(reason);
        await _database.SaveChangesAsync();
    }

    public async Task<int> CreateAsync(ReasonForSupplierChoiceDto reason)
    {
        var entity = new ReasonForSupplierChoice {Name = reason.Name};
        _database.ReasonForSupplierChoice.Add(entity);
        await _database.SaveChangesAsync();

        return entity.Id;
    }

    public async Task UpdateAsync(ReasonForSupplierChoiceDto reason)
    {
        var entity = await _database.ReasonForSupplierChoice.FirstAsync(r => r.Id == reason.Id);
        entity.Name = reason.Name;
        await _database.SaveChangesAsync();
    }
    
    public async Task MergeAsync(int targetId, int[] sourceIds)
    {
        await _database.Orders
            .Where(o => o.ReasonId.HasValue && sourceIds.Contains(o.ReasonId.Value))
            .UpdateAsync(o => new Order {ReasonId = targetId});

        var reasonsToDelete = await _database.ReasonForSupplierChoice
            .Where(m => sourceIds.Contains(m.Id))
            .ToListAsync();
        
        _database.ReasonForSupplierChoice.RemoveRange(reasonsToDelete);
        await _database.SaveChangesAsync();
    }
}