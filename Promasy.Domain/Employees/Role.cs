using System.Collections.Generic;

namespace Promasy.Domain.Employees;

public class Role
{
    public int Id { get; set; }
    public RoleName Name { get; set; }

    public ICollection<Employee> Employees = new List<Employee>();

    private Role()
    {
    }

    public Role(RoleName name)
    {
        Name = name;
    }
}