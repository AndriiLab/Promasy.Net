namespace Promasy.Modules.Core.Permissions;

public enum PermissionCondition
{
    None = 0,
    SameOrganization = 1,
    SameDepartment = 2,
    SameSubDepartment = 3,
    SameUser = 4
}