using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Finances.Dtos;

public record FinanceSubDepartmentDto(int Id, int FinanceSourceId, 
        int SubDepartmentId, string SubDepartment, int DepartmentId, string Department,
        string TotalEquipment, string TotalMaterials, string TotalServices,
        string SpentEquipment, string SpentMaterials, string SpentServices,
        string LeftEquipment, string LeftMaterials, string LeftServices,
        int EditorId = default, string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);