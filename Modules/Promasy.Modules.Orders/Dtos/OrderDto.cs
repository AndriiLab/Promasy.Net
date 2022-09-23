using Promasy.Domain.Orders;
using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Orders.Dtos;

public record OrderDto(int Id, string Description, string? CatNum,
        decimal OnePrice, decimal Amount, OrderType Type, string? Kekv, DateOnly? ProcurementStartDate,
        UnitDto Unit, CpvDto Cpv, FinanceSubDepartmentDto FinanceSubDepartment,
        SubDepartmentDto SubDepartment, DepartmentDto Department,
        ManufacturerDto? Manufacturer, SupplierDto? Supplier,
        ReasonForSupplierChoiceShortDto? Reason, int EditorId = default, string Editor = "",
        DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);

public record UnitDto(int Id, string Name);

public record CpvDto(int Id, string Code, string DescriptionEnglish, string DescriptionUkrainian, int Level,
    bool IsTerminal, int? ParentId);

public record FinanceSubDepartmentDto(int Id, int FinanceSourceId, string FinanceSource, string FinanceSourceNumber);

public record DepartmentDto(int Id, string Name);

public record SubDepartmentDto(int Id, string Name);

public record ManufacturerDto(int Id, string Name);

public record SupplierDto(int Id, string Name);

public record ReasonForSupplierChoiceShortDto(int Id, string Name);