using Promasy.Domain.Employees;
using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Employees.Dtos;

public record EmployeeShortDto(int Id, string Name, string Department, string SubDepartment, RoleName[] Roles,
        int EditorId = default, string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);