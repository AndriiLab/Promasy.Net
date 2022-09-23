using Promasy.Domain.Orders;

namespace Promasy.Modules.Orders.Dtos;

public record CreateOrderDto(string Description, string? CatNum, decimal OnePrice, decimal Amount,
    OrderType Type, string? Kekv, DateOnly? ProcurementStartDate, int UnitId, int CpvId,
    int FinanceSubDepartmentId, int? ManufacturerId, int? SupplierId, int? ReasonId);