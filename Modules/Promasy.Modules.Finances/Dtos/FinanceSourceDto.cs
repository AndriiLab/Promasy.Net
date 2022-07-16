using Promasy.Domain.Finances;
using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Finances.Dtos;

public record FinanceSourceDto(int Id, string Number, string Name, FinanceFundType FundType,
        DateOnly Start, DateOnly End, string Kpkvk,
        string TotalEquipment, string TotalMaterials, string TotalServices,
        string SpentEquipment, string SpentMaterials, string SpentServices,
        string LeftEquipment, string LeftMaterials, string LeftServices,
        int EditorId = default, string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);