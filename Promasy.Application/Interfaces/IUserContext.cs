using Promasy.Domain.Employees;

namespace Promasy.Application.Interfaces;

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
    string GetLocalizationCulture();
    DateTime AsUserTime(DateTime utcTime);
    bool HasRoles(params int[] roles);
    IReadOnlyCollection<RoleName> GetRoles();
}