using Promasy.Modules.Core.Responses;

namespace Promasy.Modules.Orders.Dtos;

public class OrderPagedResponse : PagedResponse<OrderShortDto>
{
    public decimal SpentAmount { get; set; }
    public decimal? LeftAmount { get; set; }
}