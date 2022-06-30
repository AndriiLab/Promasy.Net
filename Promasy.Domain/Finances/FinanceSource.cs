using System;
using System.Collections.Generic;
using Promasy.Core.Persistence;

namespace Promasy.Domain.Finances;

public class FinanceSource : Entity
{
    public string Name { get; set; }
    public string Number { get; set; }
    public FinanceFundType FundType { get; set; }
    public string Kpkvk { get; set; }

    public DateTime StartsOn { get; set; }
    public DateTime DueTo { get; set; }

    public decimal TotalEquipment { get; set; }
    public decimal TotalMaterials { get; set; }
    public decimal TotalServices { get; set; }

    public ICollection<FinanceDepartment> FinanceDepartments { get; set; }
}