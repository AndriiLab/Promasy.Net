using Promasy.Domain.Employees;

namespace Promasy.Modules.Employees.Dtos;

internal record CreateEmployeeDto(string FirstName, string? MiddleName, string LastName,
    string Email, string PrimaryPhone, string? ReservePhone, string UserName,
    int SubDepartmentId, RoleName[] Roles);