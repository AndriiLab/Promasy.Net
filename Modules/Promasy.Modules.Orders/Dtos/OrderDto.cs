using Promasy.Domain.Orders;
using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Orders.Dtos;

public record OrderDto(int Id, string Description, string? CatNum,
        decimal OnePrice, decimal Amount, OrderType Type, string? Kekv, DateOnly? ProcurementStartDate,
        OrderUnitDto Unit, OrderCpvDto Cpv, OrderFinanceSubDepartmentDto FinanceSubDepartment,
        OrderSubDepartmentDto SubDepartment, OrderDepartmentDto Department,
        OrderManufacturerDto? Manufacturer, OrderSupplierDto? Supplier,
        OrderReasonForSupplierChoiceShortDto? Reason, int EditorId = default, string Editor = "",
        DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);

public record OrderShortDto(int Id, string Description, decimal Total, int Status,
        int FinanceId, int SubDepartmentId,
        int EditorId = default, string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);

public record OrderSuggestionDto(int Id, string Description, string? CatNum,
    decimal OnePrice, OrderType Type, string? Kekv,
    OrderUnitDto Unit, OrderCpvDto Cpv, OrderManufacturerDto? Manufacturer,
    OrderSupplierDto? Supplier, OrderReasonForSupplierChoiceShortDto? Reason);

public record OrderUnitDto(int Id, string Name);

public record OrderCpvDto(int Id, string Code, string DescriptionEnglish, string DescriptionUkrainian, int Level,
    bool IsTerminal, int? ParentId);

public record OrderFinanceSubDepartmentDto(int Id, int FinanceSourceId, string FinanceSource, string FinanceSourceNumber);

public record OrderDepartmentDto(int Id, string Name);

public record OrderSubDepartmentDto(int Id, string Name);

public record OrderManufacturerDto(int Id, string Name);

public record OrderSupplierDto(int Id, string Name);

public record OrderReasonForSupplierChoiceShortDto(int Id, string Name);