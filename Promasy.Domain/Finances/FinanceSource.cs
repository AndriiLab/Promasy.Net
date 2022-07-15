using System;
using System.Collections.Generic;
using Promasy.Core.Persistence;

namespace Promasy.Domain.Finances;

public class FinanceSource : OrganizationEntity
{
    public string Number { get; set; }
    public string Name { get; set; }
    public FinanceFundType FundType { get; set; }
    public string Kpkvk { get; set; }

    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }

    public decimal TotalEquipment { get; set; }
    public decimal TotalMaterials { get; set; }
    public decimal TotalServices { get; set; }

    public ICollection<FinanceSubDepartment> FinanceDepartments { get; set; }
}