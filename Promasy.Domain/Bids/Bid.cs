using System;
using System.Collections.Generic;
using Promasy.Domain.Finances;
using Promasy.Domain.Producers;
using Promasy.Domain.Suppliers;
using Promasy.Domain.Vocabulary;

namespace Promasy.Domain.Bids
{
    public class Bid : Entity
    {
        public int Amount { get; set; }
        public string Description { get; set; }
        public string CatNum { get; set; }
        public decimal OnePrice { get; set; }
        public BidType Type { get; set; }
        public int? Kekv { get; set; }
        public DateTime? ProcurementStartDate { get; set; }

        public int AmountUnitId { get; set; }
        public AmountUnit AmountUnit { get; set; }

        public string CpvCode { get; set; }
        public Cpv Cpv { get; set; }

        public int FinanceDepartmentId { get; set; }
        public FinanceDepartment FinanceDepartment { get; set; }

        public int? ProducerId { get; set; }
        public Producer Producer { get; set; }

        public int ReasonId { get; set; }
        public ReasonForSupplier Reason { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public virtual ICollection<BidStatusHistory> Statuses { get; set; }
    }

    public enum BidType
    {
        Material = 1,
        Equipment = 2,
        Service = 3
    }
}