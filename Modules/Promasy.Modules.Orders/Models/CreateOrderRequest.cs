using Promasy.Domain.Orders;

namespace Promasy.Modules.Orders.Models;

public record CreateOrderRequest(string Description, string? CatNum, decimal OnePrice, int Amount,
    OrderType Type, string? Kekv, DateOnly? ProcurementStartDate, int UnitId, int CpvId,
    int FinanceSubDepartmentId, int ManufacturerId, int SupplierId, int ReasonId);