using System.Collections.Generic;
using Promasy.Persistence.Dao.Common;
using Promasy.Persistence.Dao.Commons;

namespace Promasy.Persistence.Dao.Institutes
{
    public class Department : Entity
    {
        public string Name { get; set; }
        
        public long InstituteId { get; set; }
        public virtual Institute Institute { get; set; }
        
        public virtual ICollection<Subdepartment> Subdepartments { get; set; }
    }
}
