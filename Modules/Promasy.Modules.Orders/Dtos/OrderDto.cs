using Promasy.Domain.Orders;
using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Orders.Dtos;

public record OrderDto(int Id, string Description, string? CatNum,
    decimal OnePrice, int Amount, OrderType Type, string? Kekv,
    DateOnly? ProcurementStartDate, int UnitId, string Unit,
    int CpvId, string Cpv, int FinanceSubDepartmentId, string FinanceSourceNumber,
    int FinanceId, int SubDepartmentId,
    int ManufacturerId, string Manufacturer, int SupplierId, string Supplier,
    int ReasonId, string Reason, int EditorId = default, string Editor = "", 
    DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);