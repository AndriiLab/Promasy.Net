using Promasy.Application.Interfaces;
using Promasy.Domain.Employees;
using Promasy.Domain.Orders;
using Promasy.Modules.Orders.Dtos;

namespace Promasy.Modules.Orders.Interfaces;

public interface IOrderGroupRepository : IRepository
{
    Task<string> CreateOrderGroupAsync(IEnumerable<int> orderIds, IEnumerable<Tuple<int, RoleName>> employeesWithRoles, FileType fileType);
    Task<OrderGroupDto?> GetOrderGroupByKeyAsync(string fileKey);
    Task SetGroupStatusAsync(string fileKey, OrderGroupStatus status);
}

public enum FileType
{
    Pdf = 1,
}