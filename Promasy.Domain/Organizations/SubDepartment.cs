using System.Collections.Generic;
using Promasy.Core.Persistence;
using Promasy.Domain.Finances;

namespace Promasy.Domain.Organizations;

public class SubDepartment : Entity
{
    public string Name { get; set; }
        
    public int DepartmentId { get; set; }
    public virtual Department Department { get; set; }
        
    public virtual ICollection<Employees.Employee> Employees { get; set; }
    public virtual ICollection<FinanceDepartment> FinanceDepartments { get; set; }
}

public static class SubDepartmentName
{
    public const string Default = "<відсутній>";
}