﻿using Promasy.Domain.Finances;
using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Finances.Dtos;

public record FinanceSourceDto(int Id, string Number, string Name, FinanceFundType FundType,
        DateOnly Start, DateOnly End, string Kpkvk,
        decimal TotalEquipment, decimal TotalMaterials, decimal TotalServices,
        decimal UnassignedEquipment, decimal UnassignedMaterials, decimal UnassignedServices,
        decimal LeftEquipment, decimal LeftMaterials, decimal LeftServices,
        int EditorId = default, string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);