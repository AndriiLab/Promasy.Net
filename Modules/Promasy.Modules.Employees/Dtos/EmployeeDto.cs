using Promasy.Domain.Employees;
using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Employees.Dtos;

public record EmployeeDto(int Id, string FirstName, string? MiddleName, string LastName,
        string Email, string PrimaryPhone, string? ReservePhone, int DepartmentId, string Department,
        int SubDepartmentId, string SubDepartment, RoleName[] Roles, int EditorId = default,
        string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);