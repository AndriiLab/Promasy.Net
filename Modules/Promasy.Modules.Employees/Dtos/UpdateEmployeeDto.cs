using Promasy.Domain.Employees;

namespace Promasy.Modules.Employees.Dtos;

internal record UpdateEmployeeDto(int Id, string FirstName, string? MiddleName, string LastName,
    string Email, string PrimaryPhone, string? ReservePhone, int SubDepartmentId, RoleName[] Roles);