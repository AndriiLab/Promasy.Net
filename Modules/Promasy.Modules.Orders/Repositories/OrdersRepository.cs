using Microsoft.EntityFrameworkCore;
using Promasy.Core.UserContext;
using Promasy.Domain.Orders;
using Promasy.Domain.Persistence;
using Promasy.Modules.Core.Pagination;
using Promasy.Modules.Orders.Dtos;
using Promasy.Modules.Orders.Interfaces;
using Promasy.Modules.Orders.Models;
using Z.EntityFramework.Plus;

namespace Promasy.Modules.Orders.Repositories;

internal class OrdersRepository : IOrdersRepository
{
    private readonly IDatabase _database;
    private readonly IUserContext _userContext;

    public OrdersRepository(IDatabase database, IUserContext userContext)
    {
        _database = database;
        _userContext = userContext;
    }

    public async Task<OrderPagedResponse> GetPagedListAsync(OrdersPagedRequest request)
    {
        var query = _database.Orders
            .AsNoTracking()
            .Where(r => r.Type == request.Type);

        decimal? leftAmount = null;
        query = request.FinanceSourceId.HasValue 
            ? query.Where(o => o.FinanceSubDepartment.FinanceSourceId == request.FinanceSourceId)
            : query.Where(o => o.FinanceSubDepartment.FinanceSource.Start.Year == request.Year);

        if (request.SubDepartmentId.HasValue)
        {
            query = query.Where(o => o.FinanceSubDepartment.SubDepartmentId == request.SubDepartmentId);
            if (request.FinanceSourceId.HasValue)
            {
                leftAmount = await _database.FinanceSubDepartmentsWithSpendView
                    .Where(fs => fs.SubDepartmentId == request.SubDepartmentId)
                    .SumAsync(fs =>
                        request.Type == OrderType.Material
                            ? fs.LeftMaterials
                            : request.Type == OrderType.Equipment
                                ? fs.LeftEquipment
                                : fs.LeftServices);
            }
        } 
        else if (request.DepartmentId.HasValue)
        {
            query = query.Where(o => o.FinanceSubDepartment.SubDepartment.DepartmentId == request.DepartmentId);
            if (request.FinanceSourceId.HasValue)
            {
                leftAmount = await _database.FinanceSubDepartmentsWithSpendView
                    .Where(fs => fs.SubDepartment.DepartmentId == request.DepartmentId)
                    .SumAsync(fs =>
                        request.Type == OrderType.Material
                            ? fs.LeftMaterials
                            : request.Type == OrderType.Equipment
                                ? fs.LeftEquipment
                                : fs.LeftServices);
            }
        }

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(u => EF.Functions.ToTsVector("simple", u.Description)
                                         .Matches(EF.Functions.PlainToTsQuery("simple", request.Search))  ||
                                     EF.Functions.ILike(u.CatNum, $"%{request.Search}%"));
        }

        var spentAmount = await query.SumAsync(o => o.Total);
        var response = await query
            .PaginateAsync(request,
                o => new OrderShortDto(o.Id, o.Description, o.Total.ToString("F2"),
                    (int) o.Statuses.OrderByDescending(s => s.ModifiedDate ?? s.CreatedDate).Select(s => s.Status).FirstOrDefault(),
                    o.ModifierId ?? o.CreatorId,
                    PromasyDbFunction.GetEmployeeShortName(o.ModifierId ?? o.CreatorId),
                    o.ModifiedDate ?? o.CreatedDate));

        return new OrderPagedResponse
        {
            Collection = response.Collection,
            Page = response.Page,
            Total = response.Total,
            SpentAmount = spentAmount.ToString("F2"),
            LeftAmount = leftAmount?.ToString("F2"),
        };
    }

    public Task<OrderDto?> GetByIdAsync(int id)
    {
        return _database.Orders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new OrderDto(o.Id, o.Description, o.CatNum,
                o.OnePrice, o.Amount, o.Type, o.Kekv, o.ProcurementStartDate,
                o.UnitId, o.Unit.Name, o.CpvId, o.Cpv.Code, o.FinanceSubDepartmentId,
                o.FinanceSubDepartment.FinanceSource.Number, 
                o.ManufacturerId, o.Manufacturer.Name, 
                o.SupplierId, o.Supplier.Name,
                o.ReasonId, o.Reason.Name,
                o.ModifierId ?? o.CreatorId,
                PromasyDbFunction.GetEmployeeShortName(o.ModifierId ?? o.CreatorId),
                o.ModifiedDate ?? o.CreatedDate))
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(CreateOrderDto item)
    {
        // todo: add extra validator for sum and wrap inside transaction
        var entity = new Order
        {
            Description = item.Description,
            CatNum = item.CatNum,
            OnePrice = item.OnePrice,
            Amount = item.Amount,
            Type = item.Type,
            Kekv = item.Kekv,
            ProcurementStartDate = item.ProcurementStartDate,
            UnitId = item.UnitId,
            CpvId = item.CpvId,
            FinanceSubDepartmentId = item.FinanceSubDepartmentId,
            ManufacturerId = item.ManufacturerId,
            SupplierId = item.SupplierId,
            ReasonId = item.ReasonId,
            Statuses = new List<OrderStatusHistory>
            {
                new() { Status = OrderStatus.Created }
            }
        };
        _database.Orders.Add(entity);
        
        await _database.SaveChangesAsync();

        return entity.Id;
    }

    public async Task UpdateAsync(UpdateOrderDto item)
    {
        // todo: add extra validator for sum and wrap inside transaction

        var entity = await _database.Orders.FirstOrDefaultAsync(o => o.Id == item.Id);
        if (entity is null)
        {
            return;
        }
        
        entity.Description = item.Description;
        entity.CatNum = item.CatNum;
        entity.OnePrice = item.OnePrice;
        entity.Amount = item.Amount;
        entity.Type = item.Type;
        entity.Kekv = item.Kekv;
        entity.ProcurementStartDate = item.ProcurementStartDate;
        entity.UnitId = item.UnitId;
        entity.CpvId = item.CpvId;
        entity.FinanceSubDepartmentId = item.FinanceSubDepartmentId;
        entity.ManufacturerId = item.ManufacturerId;
        entity.SupplierId = item.SupplierId;
        entity.ReasonId = item.ReasonId;
        
        await _database.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var entity = await _database.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (entity is null)
        {
            return;
        }

        await _database.OrderStatuses
            .Where(s => s.OrderId == entity.Id)
            .UpdateAsync(s => new OrderStatusHistory
                {Deleted = true, ModifiedDate = DateTime.UtcNow, ModifierId = _userContext.GetId()});

        _database.Orders.Remove(entity);
        await _database.SaveChangesAsync();
    }
}