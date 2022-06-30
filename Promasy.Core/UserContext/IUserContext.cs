using System.Collections.Generic;

namespace Promasy.Core.UserContext;

public interface IUserContext
{
    int Id { get; }
    string FirstName { get; }
    string LastName { get; }
    string Email { get; }
    string Organization { get; }
    int OrganizationId { get; }
    string Department { get; }
    int DepartmentId { get; }
    string SubDepartment { get; }
    int SubDepartmentId { get; }
    string? IpAddress { get; }
    IReadOnlyCollection<string> Roles { get; }
    bool IsAdmin();
}