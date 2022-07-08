using System.Collections.Generic;

namespace Promasy.Core.UserContext;

public interface IUserContext
{
    bool IsAuthenticated();
    int GetId();
    string GetFirstName();
    string GetLastName();
    string GetEmail();
    string GetOrganization();
    int GetOrganizationId();
    string GetDepartment();
    int GetDepartmentId();
    string GetSubDepartment();
    int GetSubDepartmentId();
    string? GetIpAddress();
    bool HasRoles(params int[] roles);
}