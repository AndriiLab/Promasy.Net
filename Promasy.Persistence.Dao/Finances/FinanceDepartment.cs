using System.Collections.Generic;
using Promasy.Persistence.Dao.Bids;
using Promasy.Persistence.Dao.Common;
using Promasy.Persistence.Dao.Commons;
using Promasy.Persistence.Dao.Institutes;

namespace Promasy.Persistence.Dao.Finances
{
    public partial class FinanceDepartment : Entity
    {
        public decimal TotalEquipment { get; set; }
        public decimal TotalMaterials { get; set; }
        public decimal TotalServices { get; set; }

        public long FinanceSourceId { get; set; }
        public FinanceSource FinanceSource { get; set; }
        
        public long SubdepartmentId { get; set; }
        public Subdepartment Subdepartment { get; set; }
        
        public virtual ICollection<Bid> Bids { get; set; }
    }
}
