namespace Promasy.Modules.Finances.Dtos;

public record CreateFinanceSubDepartmentDto(int FinanceSourceId, int SubDepartmentId,
    decimal TotalEquipment, decimal TotalMaterials, decimal TotalServices);