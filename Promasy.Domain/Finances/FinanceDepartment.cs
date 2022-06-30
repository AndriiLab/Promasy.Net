using System.Collections.Generic;
using Promasy.Core.Persistence;
using Promasy.Domain.Orders;
using Promasy.Domain.Organizations;

namespace Promasy.Domain.Finances;

public class FinanceDepartment : Entity
{
    public decimal TotalEquipment { get; set; }
    public decimal TotalMaterials { get; set; }
    public decimal TotalServices { get; set; }

    public int FinanceSourceId { get; set; }
    public FinanceSource FinanceSource { get; set; }
        
    public int SubDepartmentId { get; set; }
    public SubDepartment SubDepartment { get; set; }
        
    public virtual ICollection<Order> Bids { get; set; }
}