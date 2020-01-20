using System;
using System.Collections.Generic;
using Promasy.Persistence.Dao.Commons;
using Promasy.Persistence.Dao.Finances;
using Promasy.Persistence.Dao.Producers;
using Promasy.Persistence.Dao.Suppliers;
using Promasy.Persistence.Dao.Vocabulary;

namespace Promasy.Persistence.Dao.Bids
{
    public class Bid : Entity
    {
        public int Amount { get; set; }
        public string Description { get; set; }
        public string CatNum { get; set; }
        public decimal OnePrice { get; set; }
        public string Type { get; set; }
        public int? Kekv { get; set; }
        public DateTime ProcurementStartDate { get; set; }

        public long AmountUnitId { get; set; }
        public AmountUnit AmountUnit { get; set; }
        
        public string CpvCodeId { get; set; }
        public Cpv CpvCode { get; set; }
        
        public long FinanceDepartmentId { get; set; }

        public FinanceDepartment FinanceDepartment { get; set; }
        
        public long? ProducerId { get; set; }
        public Producer Producer { get; set; }
        
        public long ReasonId { get; set; }
        public ReasonForSupplier Reason { get; set; }
        
        public long SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        
        public virtual ICollection<BidStatus> BidStatuses { get; set; }
    }
}
