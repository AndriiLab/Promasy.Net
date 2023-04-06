using System.Collections.Generic;
using Promasy.Core.Persistence;
using Promasy.Domain.Employees;
using RoleName = Promasy.Domain.Employees.RoleName;

namespace Promasy.Domain.Orders;

public class OrderGroup : OrganizationEntity
{
    public string FileKey { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<OrderGroupEmployee> Employees { get; set; } = new List<OrderGroupEmployee>();
    public OrderGroupStatus Status { get; set; }
}

public enum OrderGroupStatus
{
    Created = 1,
    FileGenerated = 2,
}

public class OrderGroupEmployee : Entity
{
    public RoleName Role { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public int OrderGroupId { get; set; }
    public OrderGroup OrderGroup { get; set; }
}