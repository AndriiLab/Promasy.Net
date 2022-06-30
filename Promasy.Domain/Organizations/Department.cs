using System.Collections.Generic;
using Promasy.Core.Persistence;

namespace Promasy.Domain.Organizations;

public class Department : Entity
{
    public string Name { get; set; }
        
    public int OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }
        
    public virtual ICollection<SubDepartment> SubDepartments { get; set; }
}