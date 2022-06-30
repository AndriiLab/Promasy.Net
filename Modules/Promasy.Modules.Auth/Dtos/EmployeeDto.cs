namespace Promasy.Modules.Auth.Dtos;

internal record EmployeeDto(int Id, string FirstName, string MiddleName, string LastName, string Email,
    string Organization, int OrganizationId, string Department, int DepartmentId,
    string SubDepartment, int SubDepartmentId, string[] Roles);