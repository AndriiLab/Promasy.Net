using Promasy.Domain.Employees;

namespace Promasy.Modules.Orders.Dtos;

internal class EmployeeDto
{
    public EmployeeDto(int id, string name, string phone, int departmentId)
    {
        Id = id;
        Name = name;
        Phone = phone;
        DepartmentId = departmentId;
    }
    
    public EmployeeDto(int id, string name, string phone, int departmentId, RoleName role)
    {
        Id = id;
        Name = name;
        Phone = phone;
        Role = role;
        DepartmentId = departmentId;
    }

    public int Id { get; init; }
    public string Name { get; init; }
    public string Phone { get; init; }
    public int DepartmentId { get; init; }
    public RoleName Role { get; set; }
}