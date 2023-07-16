using Promasy.Core.Persistence;
using Promasy.Domain.Finances;
using Promasy.Domain.Organizations;

namespace Promasy.Application.Persistence.Views;

public class FinanceSubDepartmentsView : Entity
{
    public int FinanceSourceId { get; set; }
    public FinanceSource FinanceSource { get; set; }
    public int SubDepartmentId { get; set; }
    public SubDepartment SubDepartment { get; set; }
    
    public decimal TotalEquipment { get; set; }
    public decimal LeftEquipment { get; set; }
    public decimal TotalMaterials { get; set; }
    public decimal LeftMaterials { get; set; }
    public decimal TotalServices { get; set; }
    public decimal LeftServices { get; set; }
}