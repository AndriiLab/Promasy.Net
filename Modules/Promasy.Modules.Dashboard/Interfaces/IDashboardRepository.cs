using Promasy.Application.Interfaces;
using Promasy.Domain.Orders;
using Promasy.Modules.Dashboard.Dtos;

namespace Promasy.Modules.Dashboard.Interfaces;

internal interface IDashboardRepository : IRepository
{
    Task<DashboardCountsDto<int>> GetOrdersCountAsync(int year);
    Task<DashboardCountsDto<decimal>> GetFundingLeftAsync(OrderType type, int year);
}