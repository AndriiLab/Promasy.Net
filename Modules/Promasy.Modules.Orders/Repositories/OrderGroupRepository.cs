using Microsoft.EntityFrameworkCore;
using Promasy.Domain.Employees;
using Promasy.Domain.Orders;
using Promasy.Domain.Persistence;
using Promasy.Modules.Orders.Dtos;
using Promasy.Modules.Orders.Interfaces;

namespace Promasy.Modules.Orders.Repositories;

public class OrderGroupRepository : IOrderGroupRepository
{
    private readonly IDatabase _database;

    public OrderGroupRepository(IDatabase database)
    {
        _database = database;
    }

    public async Task<string> CreateOrderGroupAsync(IEnumerable<int> orderIds,
        IEnumerable<Tuple<int, RoleName>> employeesWithRoles, FileType fileType)
    {
        var orders = await _database.Orders.Where(o => orderIds.Contains(o.Id)).ToListAsync();
        var og = new OrderGroup
        {
            FileKey = $"{Guid.NewGuid():N}.{fileType.ToString().ToLower()}",
            Orders = orders,
            Employees = employeesWithRoles.Select(e => new OrderGroupEmployee
                {
                    EmployeeId = e.Item1,
                    Role = e.Item2,
                })
                .ToList(),
            Status = OrderGroupStatus.Created
        };
        _database.OrderGroups.Add(og);
        await _database.SaveChangesAsync();

        return og.FileKey;
    }

    public Task<OrderGroupDto?> GetOrderGroupByKeyAsync(string fileKey)
    {
        return _database.OrderGroups
            .AsNoTracking()
            .Where(og => og.FileKey == fileKey)
            .Select(og => new OrderGroupDto(og.Id, og.FileKey, og.Status,
                og.Orders.Select(o => o.Id).ToList(),
                og.Employees.Select(oe => new OrderGroupEmployeeDto(oe.EmployeeId, oe.Role)).ToList(),
                og.ModifierId ?? og.CreatorId,
                PromasyDbFunction.GetEmployeeShortName(og.ModifierId ?? og.CreatorId),
                og.ModifiedDate ?? og.CreatedDate))
            .FirstOrDefaultAsync();
    }

    public async Task SetGroupStatusAsync(string fileKey, OrderGroupStatus status)
    {
        var og = await _database.OrderGroups
            .Where(og => og.FileKey == fileKey)
            .FirstOrDefaultAsync();
        if (og is null)
        {
            return;
        }

        og.Status = status;
        await _database.SaveChangesAsync();
    }
}