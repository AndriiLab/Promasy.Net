using System.Collections.Generic;
using Promasy.Core.Persistence;

namespace Promasy.Domain.Organizations;

public class Organization : Entity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Edrpou { get; set; }
    public string FaxNumber { get; set; }
    public string PhoneNumber { get; set; }

    public int AddressId { get; set; }
    public virtual Address Address { get; set; }
        
    public virtual ICollection<Department> Departments { get; set; }
}