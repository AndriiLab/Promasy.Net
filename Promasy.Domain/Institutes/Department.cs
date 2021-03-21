using System.Collections.Generic;
using Promasy.Common.Persistence;

namespace Promasy.Domain.Institutes
{
    public class Department : Entity
    {
        public string Name { get; set; }
        
        public int InstituteId { get; set; }
        public virtual Institute Institute { get; set; }
        
        public virtual ICollection<SubDepartment> SubDepartments { get; set; }
    }
}
