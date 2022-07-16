using System;
using Promasy.Core.Persistence;
using Promasy.Domain.Finances;

namespace Promasy.Domain.Persistence.Views;

public class FinanceSourceView : OrganizationEntity
{
    public string Number { get; set; }
    public string Name { get; set; }
    public FinanceFundType FundType { get; set; }
    public string Kpkvk { get; set; }

    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }

    public decimal TotalEquipment { get; set; }
    public decimal SpentEquipment { get; set; } // todo: possibly not used
    public decimal LeftEquipment { get; set; }
    public decimal TotalMaterials { get; set; }
    public decimal SpentMaterials { get; set; }
    public decimal LeftMaterials { get; set; }
    public decimal TotalServices { get; set; }
    public decimal SpentServices { get; set; }
    public decimal LeftServices { get; set; }

}