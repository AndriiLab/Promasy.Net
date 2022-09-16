using Promasy.Domain.Orders;
using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Orders.Interfaces;

internal interface IOrderRules : IRules<Order>
{
    Task<bool> IsCpvCanBeUsedAsync(int cpvId, CancellationToken ct);
    Task<bool> IsSufficientFundsAsync(int financeSubDepartmentId, decimal total, OrderType type, CancellationToken ct);
    Task<bool> IsSufficientFundsAsync(int financeSubDepartmentId, int id, decimal total, OrderType type, CancellationToken ct);
}