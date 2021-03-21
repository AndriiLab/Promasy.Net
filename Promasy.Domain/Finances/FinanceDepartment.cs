using System.Collections.Generic;
using Promasy.Common.Persistence;
using Promasy.Domain.Bids;
using Promasy.Domain.Institutes;

namespace Promasy.Domain.Finances
{
    public class FinanceDepartment : Entity
    {
        public decimal TotalEquipment { get; set; }
        public decimal TotalMaterials { get; set; }
        public decimal TotalServices { get; set; }

        public int FinanceSourceId { get; set; }
        public FinanceSource FinanceSource { get; set; }
        
        public int SubDepartmentId { get; set; }
        public SubDepartment SubDepartment { get; set; }
        
        public virtual ICollection<Bid> Bids { get; set; }
    }
}
