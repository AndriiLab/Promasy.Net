using Promasy.Modules.Core.Modules;
using Promasy.Modules.Orders.Dtos;
using Promasy.Modules.Orders.Models;

namespace Promasy.Modules.Orders.Interfaces;

internal interface IOrdersRepository : IRepository
{
    Task<OrderPagedResponse> GetPagedListAsync(OrdersPagedRequest request);
    Task<OrderDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateOrderDto item);
    Task UpdateAsync(UpdateOrderDto item);
    Task DeleteByIdAsync(int id);
}