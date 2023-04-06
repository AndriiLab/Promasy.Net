using Promasy.Application.Interfaces;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Orders.Dtos;
using Promasy.Modules.Orders.Models;

namespace Promasy.Modules.Orders.Interfaces;

internal interface IOrdersRepository : IRepository
{
    Task<OrderPagedResponse> GetPagedListAsync(OrdersPagedRequest request);
    Task<PagedResponse<OrderSuggestionDto>> GetOrderSuggestionsPagedListAsync(OrderSuggestionPagedRequest request);
    Task<OrderDto?> GetByIdAsync(int id);
    Task<List<OrderDto>> GetByIdsAsync(IEnumerable<int> ids);
    Task<int> CreateAsync(CreateOrderDto item);
    Task UpdateAsync(UpdateOrderDto item);
    Task DeleteByIdAsync(int id);
}