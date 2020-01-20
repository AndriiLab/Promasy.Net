using System.Collections.Generic;
using Promasy.Persistence.Dao.Common;
using Promasy.Persistence.Dao.Commons;
using Promasy.Persistence.Dao.Finances;
using Promasy.Persistence.Dao.Users;

namespace Promasy.Persistence.Dao.Institutes
{
    public class Subdepartment : Entity
    {
        public string Name { get; set; }
        
        public long? DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<FinanceDepartment> FinanceDepartments { get; set; }
    }
}
