using Promasy.Domain.Employees;

namespace Promasy.Modules.Orders.Dtos;

internal record EmployeeDto(int Id, string Name, string Phone, int DepartmentId, RoleName Role);