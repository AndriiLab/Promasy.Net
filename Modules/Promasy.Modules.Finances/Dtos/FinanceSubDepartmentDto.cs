using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Finances.Dtos;

public record FinanceSubDepartmentDto(int Id, int FinanceSourceId, string FinanceSource,
        string FinanceSourceNumber, int SubDepartmentId, string SubDepartment, int DepartmentId, string Department,
        decimal TotalEquipment, decimal TotalMaterials, decimal TotalServices,
        decimal LeftEquipment, decimal LeftMaterials, decimal LeftServices,
        int EditorId = default, string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);