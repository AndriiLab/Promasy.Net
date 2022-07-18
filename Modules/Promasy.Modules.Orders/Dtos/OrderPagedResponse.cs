using Promasy.Modules.Core.Responses;

namespace Promasy.Modules.Orders.Dtos;

public class OrderPagedResponse : PagedResponse<OrderShortDto>
{
    public string SpentAmount { get; set; }
    public string? LeftAmount { get; set; }
}