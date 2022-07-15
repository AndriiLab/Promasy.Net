using Promasy.Domain.Finances;

namespace Promasy.Modules.Finances.Dtos;

public record UpdateFinanceSourceDto(int Id, string Number, string Name, FinanceFundType FundType,
    DateOnly Start, DateOnly End, string Kpkvk,
    decimal TotalEquipment, decimal TotalMaterials, decimal TotalServices);