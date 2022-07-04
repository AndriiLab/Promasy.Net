using System;
using System.Collections.Generic;
using Promasy.Core.Persistence;
using Promasy.Domain.Finances;
using Promasy.Domain.Manufacturers;
using Promasy.Domain.Suppliers;
using Promasy.Domain.Vocabulary;

namespace Promasy.Domain.Orders;

public class Order : Entity
{
    public int Amount { get; set; }
    public string Description { get; set; }
    public string? CatNum { get; set; }
    public decimal OnePrice { get; set; }
    public OrderType Type { get; set; }
    public string? Kekv { get; set; }
    public DateTime? ProcurementStartDate { get; set; }

    public int UnitId { get; set; }
    public Unit Unit { get; set; }

    public int CpvId { get; set; }
    public Cpv Cpv { get; set; }

    public int FinanceDepartmentId { get; set; }
    public FinanceDepartment FinanceDepartment { get; set; }

    public int ManufacturerId { get; set; }
    public Manufacturer Manufacturer { get; set; }

    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; }
    
    public int ReasonId { get; set; }
    public ReasonForSupplierChoice Reason { get; set; }

    public virtual ICollection<OrderStatusHistory> Statuses { get; set; }
}