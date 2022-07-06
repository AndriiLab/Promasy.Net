using System.Collections.Generic;
using Promasy.Core.Persistence;

namespace Promasy.Domain.Organizations;

public class Department : OrganizationEntity
{
    public string Name { get; set; }
    public virtual Organization Organization { get; set; }
        
    public virtual ICollection<SubDepartment> SubDepartments { get; set; }
}