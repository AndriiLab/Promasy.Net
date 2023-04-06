using Microsoft.EntityFrameworkCore;
using Promasy.Application.Interfaces;
using Promasy.Domain.Finances;
using Promasy.Domain.Orders;
using Promasy.Domain.Persistence;
using Promasy.Modules.Core.Pagination;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Finances.Dtos;
using Promasy.Modules.Finances.Interfaces;
using Promasy.Modules.Finances.Models;
using Z.EntityFramework.Plus;

namespace Promasy.Modules.Finances.Repositories;

internal class FinanceSourcesRepository : IFinanceSourceRules, IFinanceSourcesRepository
{
    private readonly IDatabase _database;
    private readonly IUserContext _userContext;

    public FinanceSourcesRepository(IDatabase database, IUserContext userContext)
    {
        _database = database;
        _userContext = userContext;
    }
    
    public Task<bool> IsExistsAsync(int id, CancellationToken ct)
    {
        return _database.FinanceSources.AnyAsync(s => s.Id == id, ct);
    }

    public async Task<bool> IsNumberUniqueAsync(string number, int year, CancellationToken ct)
    {
        return await _database.FinanceSources
            .Where(s => s.Start.Year == year)
            .AnyAsync(s => EF.Functions.ILike(s.Number, number), ct) == false;
    }

    public async Task<bool> IsNumberUniqueAsync(string number, int year, int id, CancellationToken ct)
    {
        return await _database.FinanceSources
            .Where(s => s.Start.Year == year && s.Id != id)
            .AnyAsync(s => EF.Functions.ILike(s.Number, number), ct) == false;
    }

    public Task<PagedResponse<FinanceSourceShortDto>> GetPagedListAsync(FinanceSourcesPagedRequest request)
    {
        return request.IncludeCalculatedAmounts
            ? GetPagedListExtendedAsync(request)
            : GetPagedListShortAsync(request);
    }

    private Task<PagedResponse<FinanceSourceShortDto>> GetPagedListShortAsync(FinanceSourcesPagedRequest request)
    {
        var query = _database.FinanceSources
            .AsNoTracking()
            .Where(f => f.Start.Year == request.Year);
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(u => EF.Functions.ILike(u.Name, $"%{request.Search}%") ||
                                     EF.Functions.ILike(u.Number, $"%{request.Search}%"));
        }

        return query
            .PaginateAsync(request,
                f => new FinanceSourceShortDto(f.Id, f.Number, f.Name, f.FundType, f.Start, f.End,
                    f.Kpkvk, f.TotalEquipment, f.TotalMaterials, f.TotalServices,
                    decimal.Zero, decimal.Zero, decimal.Zero, 
                    f.ModifierId ?? f.CreatorId,
                    PromasyDbFunction.GetEmployeeShortName(f.ModifierId ?? f.CreatorId),
                    f.ModifiedDate ?? f.CreatedDate));
    }

    private Task<PagedResponse<FinanceSourceShortDto>> GetPagedListExtendedAsync(FinanceSourcesPagedRequest request)
    {
        var query = _database.FinanceSourceWithSpendView
            .AsNoTracking()
            .Where(f => f.Start.Year == request.Year);
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(u => EF.Functions.ILike(u.Name, $"%{request.Search}%") ||
                                     EF.Functions.ILike(u.Number, $"%{request.Search}%"));
        }

        return query
            .PaginateAsync(request,
                f => new FinanceSourceShortDto(f.Id, f.Number, f.Name, f.FundType, f.Start, f.End,
                    f.Kpkvk, f.TotalEquipment, f.TotalMaterials, f.TotalServices,
                    f.LeftEquipment, f.LeftMaterials, f.LeftServices, 
                    f.ModifierId ?? f.CreatorId,
                    PromasyDbFunction.GetEmployeeShortName(f.ModifierId ?? f.CreatorId),
                    f.ModifiedDate ?? f.CreatedDate));
    }

    public Task<FinanceSourceDto?> GetByIdAsync(int id)
    {
        return _database.FinanceSourceWithSpendView
            .AsNoTracking()
            .Where(f => f.Id == id)
            .Select(f => new FinanceSourceDto(f.Id, f.Number, f.Name, f.FundType, f.Start, f.End,
                f.Kpkvk, f.TotalEquipment, f.TotalMaterials, f.TotalServices,
                f.UnassignedEquipment, f.UnassignedMaterials, f.UnassignedServices,
                f.LeftEquipment, f.LeftMaterials, f.LeftServices,
                f.ModifierId ?? f.CreatorId, PromasyDbFunction.GetEmployeeShortName(f.ModifierId ?? f.CreatorId),
                f.ModifiedDate ?? f.CreatedDate))
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(CreateFinanceSourceDto item)
    {
        var entity = new FinanceSource
        {
            Number = item.Number,
            Name = item.Name,
            Start = item.Start,
            End = item.End,
            Kpkvk = item.Kpkvk,
            FundType = item.FundType,
            TotalEquipment = item.TotalEquipment,
            TotalMaterials = item.TotalMaterials,
            TotalServices = item.TotalServices,
        };

        _database.FinanceSources.Add(entity);

        await _database.SaveChangesAsync();

        return entity.Id;
    }

    public async Task UpdateAsync(UpdateFinanceSourceDto item)
    {
        var entity = await _database.FinanceSources.FirstOrDefaultAsync(f => f.Id == item.Id);
        if (entity is null)
        {
            return;
        }
        
        entity.Number = item.Number;
        entity.Name = item.Name;
        entity.Start = item.Start;
        entity.End = item.End;
        entity.Kpkvk = item.Kpkvk;
        entity.FundType = item.FundType;
        entity.TotalEquipment = item.TotalEquipment;
        entity.TotalMaterials = item.TotalMaterials;
        entity.TotalServices = item.TotalServices;

        await _database.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        await using var trx = await _database.BeginTransactionAsync();
        try
        {
            var entity = await _database.FinanceSources.FirstOrDefaultAsync(f => f.Id == id);
            if (entity is null)
            {
                await trx.RollbackAsync();
                return;
            }

            await _database.Orders
                .Where(o => o.FinanceSubDepartment.FinanceSourceId == id)
                .UpdateAsync(o => new Order
                    {Deleted = true, ModifiedDate = DateTime.UtcNow, ModifierId = _userContext.GetId()});
        
            await _database.FinanceSubDepartments
                .Where(f => f.FinanceSourceId == id)
                .UpdateAsync(o => new FinanceSubDepartment
                    {Deleted = true, ModifiedDate = DateTime.UtcNow, ModifierId = _userContext.GetId()});

            await _database.OrderStatuses
                .Where(s => s.Order.FinanceSubDepartment.FinanceSourceId == entity.Id)
                .UpdateAsync(s => new OrderStatusHistory
                    {Deleted = true, ModifiedDate = DateTime.UtcNow, ModifierId = _userContext.GetId()});
            
            _database.FinanceSources.Remove(entity);
            await _database.SaveChangesAsync();

            await trx.CommitAsync();
        }
        catch (Exception)
        {
            await trx.RollbackAsync();
        }
    }
}