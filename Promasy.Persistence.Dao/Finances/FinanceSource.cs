using System;
using System.Collections.Generic;
using Promasy.Persistence.Dao.Common;
using Promasy.Persistence.Dao.Commons;

namespace Promasy.Persistence.Dao.Finances
{
    public partial class FinanceSource : Entity
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Fundtype { get; set; }
        public int Kpkvk { get; set; }

        public DateTime StartsOn { get; set; }
        public DateTime DueTo { get; set; }

        public decimal TotalEquipment { get; set; }
        public decimal TotalMaterials { get; set; }
        public decimal TotalServices { get; set; }

        public ICollection<FinanceDepartment> FinanceDepartments { get; set; }
    }
}