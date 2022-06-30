using Promasy.Core.UserContext;
using Promasy.Domain.Employees;

namespace Promasy.Modules.Core.UserContext;

public class UserContext : IUserContext
{
    public int Id { get; }
    public string FirstName { get; }
    public string MiddleName { get; }
    public string LastName { get; }
    public string Email { get; }
    public string Organization { get; }
    public int OrganizationId { get; }
    public string Department { get; }
    public int DepartmentId { get; }
    public string SubDepartment { get; }
    public string? IpAddress { get; }
    public int SubDepartmentId { get; }
    public IReadOnlyCollection<string> Roles { get; }
    public bool IsAdmin() => Roles.Contains(RoleName.Administrator);

    public UserContext(int id, string firstName, string middleName, string lastName, string email, string organization,
        int organizationId, string department, int departmentId, string subDepartment, int subDepartmentId, string? ipAddress,
        IReadOnlyCollection<string> roles)
    {
        Id = id;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Email = email;
        Organization = organization;
        OrganizationId = organizationId;
        Department = department;
        DepartmentId = departmentId;
        SubDepartment = subDepartment;
        SubDepartmentId = subDepartmentId;
        IpAddress = ipAddress;
        Roles = roles;
    }
}