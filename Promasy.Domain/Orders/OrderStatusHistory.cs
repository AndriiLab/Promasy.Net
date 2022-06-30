using Promasy.Core.Persistence;

namespace Promasy.Domain.Orders;

public class OrderStatusHistory : Entity
{
    public OrderStatus Status { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; }
}