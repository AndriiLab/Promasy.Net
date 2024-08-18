using Microsoft.EntityFrameworkCore;
using Promasy.Application.Helpers;
using Promasy.Application.Interfaces;
using Promasy.Application.Persistence;
using Promasy.Domain.Finances;
using Promasy.Modules.Core.Pagination;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Finances.Dtos;
using Promasy.Modules.Finances.Interfaces;
using Promasy.Modules.Finances.Models;

namespace Promasy.Modules.Finances.Repositories;

internal class FinanceSubDepartmentsRepository : IFinanceSubDepartmentRules, IFinanceSubDepartmentsRepository
{
    private readonly IDatabase _database;
    private readonly IUserContext _userContext;

    public FinanceSubDepartmentsRepository(IDatabase database, IUserContext userContext)
    {
        _database = database;
        _userContext = userContext;
    }
    
    public Task<bool> IsExistsAsync(int id, CancellationToken ct)
    {
        return _database.FinanceSubDepartments.AnyAsync(s => s.Id == id, ct);
    }

    public Task<bool> IsUniqueFinanceSubDepartmentAsync(int financeSourceId, int subDepartmentId, CancellationToken ct)
    {
        return _database.FinanceSubDepartments
            .Where(s => s.FinanceSourceId == financeSourceId)
            .AllAsync(s => s.SubDepartmentId != subDepartmentId, ct);
    }

    public Task<bool> IsUniqueFinanceSubDepartmentAsync(int id, int financeSourceId, int subDepartmentId, CancellationToken ct)
    {
        return _database.FinanceSubDepartments
            .Where(s => s.FinanceSourceId == financeSourceId && s.Id != id)
            .AllAsync(s => s.SubDepartmentId != subDepartmentId, ct);
    }

    public Task<bool> CanBeAssignedAsEquipmentAsync(decimal amount, int financeSourceId, CancellationToken ct)
    {
        return _database.FinanceSources
            .Where(s => s.Id == financeSourceId)
            .AnyAsync(s => s.TotalEquipment - s.FinanceDepartments
                .Where(d => d.Deleted == false) // todo: bug in EF Core
                .Sum(d => d.TotalEquipment) - amount >= 0, ct);
    }

    public Task<bool> CanBeAssignedAsEquipmentAsync(decimal amount, int id, int financeSourceId, CancellationToken ct)
    {
        return _database.FinanceSources
            .Where(s => s.Id == financeSourceId)
            .AnyAsync(s => s.TotalEquipment - s.FinanceDepartments
                .Where(d => d.Deleted == false) // todo: bug in EF Core
                .Where(d => d.Id != id)
                .Sum(d => d.TotalEquipment) - amount >= 0, ct);
    }

    public Task<bool> CanBeAssignedAsMaterialsAsync(decimal amount, int financeSourceId, CancellationToken ct)
    {
        return _database.FinanceSources
            .Where(s => s.Id == financeSourceId)
            .AnyAsync(s => s.TotalMaterials - s.FinanceDepartments
                .Where(d => d.Deleted == false) // todo: bug in EF Core
                .Sum(d => d.TotalMaterials) - amount >= 0, ct);
    }

    public Task<bool> CanBeAssignedAsMaterialsAsync(decimal amount, int id, int financeSourceId, CancellationToken ct)
    {
        return _database.FinanceSources
            .Where(s => s.Id == financeSourceId)
            .AnyAsync(s => s.TotalMaterials - s.FinanceDepartments
                .Where(d => d.Deleted == false) // todo: bug in EF Core
                .Where(d => d.Id != id)
                .Sum(d => d.TotalMaterials) - amount >= 0, ct);
    }

    public Task<bool> CanBeAssignedAsServicesAsync(decimal amount, int financeSourceId, CancellationToken ct)
    {
        return _database.FinanceSources
            .Where(s => s.Id == financeSourceId)
            .AnyAsync(s => s.TotalServices - s.FinanceDepartments
                .Where(d => d.Deleted == false) // todo: bug in EF Core
                .Sum(d => d.TotalServices) - amount >= 0, ct);
    }

    public Task<bool> CanBeAssignedAsServicesAsync(decimal amount, int id, int financeSourceId, CancellationToken ct)
    {
        return _database.FinanceSources
            .Where(s => s.Id == financeSourceId)
            .AnyAsync(s => s.TotalServices - s.FinanceDepartments
                .Where(d => d.Deleted == false) // todo: bug in EF Core
                .Where(d => d.Id != id)
                .Sum(d => d.TotalServices) - amount >= 0, ct);
    }

    public Task<PagedResponse<FinanceSubDepartmentDto>> GetPagedListAsync(int financeSourceId, PagedRequest request)
    {
        var query = _database.FinanceSubDepartmentsWithSpendView
            .AsNoTracking()
            .Where(f => f.FinanceSourceId == financeSourceId);

        return query
            .PaginateAsync(request,
                f => new FinanceSubDepartmentDto(f.Id, f.FinanceSourceId, f.FinanceSource.Name, f.FinanceSource.Number,
                    f.SubDepartmentId, f.SubDepartment.Name, f.SubDepartment.DepartmentId, f.SubDepartment.Department.Name,
                    f.TotalEquipment, f.TotalMaterials, f.TotalServices, 
                    f.LeftEquipment, f.LeftMaterials, f.LeftServices, 
                    f.ModifierId ?? f.CreatorId,
                    PromasyDbFunction.GetEmployeeShortName(f.ModifierId ?? f.CreatorId),
                    f.ModifiedDate ?? f.CreatedDate));
    }

    public Task<PagedResponse<FinanceSubDepartmentDto>> GetPagedListBySubDepartmentAsync(GetFinanceSubDepartmentsPagedRequest request)
    {
        var query = _database.FinanceSubDepartmentsWithSpendView
            .AsNoTracking()
            .Where(f => f.SubDepartmentId == request.SubDepartmentId && (f.FinanceSource.Start.Year == request.Year || f.FinanceSource.End.Year == request.Year));

        return query
            .PaginateAsync(request,
                f => new FinanceSubDepartmentDto(f.Id, f.FinanceSourceId, f.FinanceSource.Name, f.FinanceSource.Number,
                    f.SubDepartmentId, f.SubDepartment.Name, f.SubDepartment.DepartmentId, f.SubDepartment.Department.Name,
                    f.TotalEquipment, f.TotalMaterials, f.TotalServices, 
                    f.LeftEquipment, f.LeftMaterials, f.LeftServices, 
                    f.ModifierId ?? f.CreatorId,
                    PromasyDbFunction.GetEmployeeShortName(f.ModifierId ?? f.CreatorId),
                    f.ModifiedDate ?? f.CreatedDate));
    }

    public Task<FinanceSubDepartmentDto?> GetByFinanceSubDepartmentIdsAsync(int financeId, int subDepartmentId)
    {
        return _database.FinanceSubDepartmentsWithSpendView
            .Where(fs => fs.FinanceSourceId == financeId && fs.SubDepartmentId == subDepartmentId)
            .Select(f => new FinanceSubDepartmentDto(f.Id, f.FinanceSourceId, f.FinanceSource.Name,
                f.FinanceSource.Number, f.SubDepartmentId, f.SubDepartment.Name,
                f.SubDepartment.DepartmentId, f.SubDepartment.Department.Name,
                f.TotalEquipment, f.TotalMaterials, f.TotalServices,
                f.LeftEquipment, f.LeftMaterials, f.LeftServices, 
                f.ModifierId ?? f.CreatorId,
                PromasyDbFunction.GetEmployeeShortName(f.ModifierId ?? f.CreatorId),
                f.ModifiedDate ?? f.CreatedDate))
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(CreateFinanceSubDepartmentDto item)
    {
        var entity = new FinanceSubDepartment
        {
            TotalEquipment = item.TotalEquipment,
            TotalMaterials = item.TotalMaterials,
            TotalServices = item.TotalServices,
            FinanceSourceId = item.FinanceSourceId,
            SubDepartmentId = item.SubDepartmentId
        };

        _database.FinanceSubDepartments.Add(entity);

        await _database.SaveChangesAsync();

        return entity.Id;
    }

    public async Task UpdateAsync(UpdateFinanceSubDepartmentDto item)
    {
        var entity = await _database.FinanceSubDepartments
            .FirstOrDefaultAsync(fs => fs.Id == item.Id);
        if (entity is null)
        {
            return;
        }
        
        entity.TotalEquipment = item.TotalEquipment;
        entity.TotalMaterials = item.TotalMaterials;
        entity.TotalServices = item.TotalServices;
        entity.FinanceSourceId = item.FinanceSourceId;
        entity.SubDepartmentId = item.SubDepartmentId;
        await _database.SaveChangesAsync();
    }

    public async Task DeleteByFinanceSubDepartmentIdsAsync(int financeId, int subDepartmentId)
    {
        await using var trx = await _database.BeginTransactionAsync();
        try
        {
            var entities = await _database.FinanceSubDepartments
                .Where(fs => fs.FinanceSourceId == financeId && fs.SubDepartmentId ==subDepartmentId)
                .ToListAsync();
            if (entities.Count == 0)
            {
                await trx.RollbackAsync();
                return;
            }

            await _database.Orders
                .Where(o => entities.Select(e => e.Id).Contains(o.FinanceSubDepartmentId))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(e => e.Deleted, true)
                    .SetProperty(e => e.ModifiedDate, DateTime.UtcNow)
                    .SetProperty(e => e.ModifierId, _userContext.GetId()));

            await _database.OrderStatuses
                .Where(s => entities.Select(e => e.Id).Contains(s.Order.FinanceSubDepartmentId))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(e => e.Deleted, true)
                    .SetProperty(e => e.ModifiedDate, DateTime.UtcNow)
                    .SetProperty(e => e.ModifierId, _userContext.GetId()));
            
            _database.FinanceSubDepartments.RemoveRange(entities);
            await _database.SaveChangesAsync();

            await trx.CommitAsync();
        }
        catch (Exception)
        {
            await trx.RollbackAsync();
        }
    }

    public Task<bool> IsSameOrganizationAsync(int id, int userOrganizationId, CancellationToken ct)
    {
        return _database.FinanceSubDepartments.AsNoTracking().AnyAsync(fs => fs.Id == id && fs.SubDepartment.OrganizationId == userOrganizationId, ct);
    }

    public Task<bool> IsSameDepartmentAsync(int id, int userDepartmentId, CancellationToken ct)
    {
        return _database.FinanceSubDepartments.AsNoTracking().AnyAsync(fs => fs.Id == id && fs.SubDepartment.DepartmentId == userDepartmentId, ct);
    }

    public Task<bool> IsSameSubDepartmentAsync(int id, int userSubDepartmentId, CancellationToken ct)
    {
        return _database.FinanceSubDepartments.AsNoTracking().AnyAsync(fs => fs.Id == id && fs.SubDepartmentId == userSubDepartmentId, ct);
    }

    public Task<bool> IsSameUserAsync(int id, int userId, CancellationToken ct)
    {
        return PermissionRulesRepositoryHelper.IsSameUserAsync<FinanceSubDepartment>(_database, id, userId, ct);
    }
}