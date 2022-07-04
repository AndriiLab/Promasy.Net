using System.Collections.Generic;
using Promasy.Core.Persistence;
using Promasy.Domain.Organizations;

namespace Promasy.Domain.Employees;

public class Employee : Entity
{
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PrimaryPhone { get; set; }
    public string? ReservePhone { get; set; }
    public string Password { get; set; }
    public long? Salt { get; set; }

    public int SubDepartmentId { get; set; }
    public virtual SubDepartment SubDepartment { get; set; }

    public ICollection<Role> Roles = new List<Role>();
}