using System.Collections.Generic;
using Promasy.Domain.Finances;
using Promasy.Domain.Users;

namespace Promasy.Domain.Institutes
{
    public class SubDepartment : Entity
    {
        public string Name { get; set; }
        
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<FinanceDepartment> FinanceDepartments { get; set; }
    }

    public static class SubDepartmentName
    {
        public const string Default = "<відсутній>";
    }
}
