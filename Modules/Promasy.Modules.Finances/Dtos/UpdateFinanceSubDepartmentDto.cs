namespace Promasy.Modules.Finances.Dtos;

public record UpdateFinanceSubDepartmentDto(int Id, int FinanceSourceId, int SubDepartmentId,
    decimal TotalEquipment, decimal TotalMaterials, decimal TotalServices);