using Promasy.Core.Persistence;
using Promasy.Domain.Finances;
using Promasy.Domain.Organizations;

namespace Promasy.Domain.Persistence.Views;

public class FinanceSubDepartmentsWithSpend : OrganizationEntity
{
    public int FinanceSourceId { get; set; }
    public FinanceSource FinanceSource { get; set; }
    public int SubDepartmentId { get; set; }
    public SubDepartment SubDepartment { get; set; }
    
    public decimal TotalEquipment { get; set; }
    public decimal SpentEquipment { get; set; }
    public decimal TotalMaterials { get; set; }
    public decimal SpentMaterials { get; set; }
    public decimal TotalServices { get; set; }
    public decimal SpentServices { get; set; }
}