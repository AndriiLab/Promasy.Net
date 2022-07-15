using Promasy.Domain.Employees;
using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Core.Rules;

public interface IEmployeeRules : IRules
{
    bool CanChangePasswordForEmployee(int id);
    bool IsEditable(int id);
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken ct);
    Task<bool> IsEmailUniqueAsync(string email, int id, CancellationToken ct);
    Task<bool> IsPhoneUniqueAsync(string phone, CancellationToken ct);
    Task<bool> IsPhoneUniqueAsync(string phone, int id, CancellationToken ct);
    Task<bool> IsUserNameUniqueAsync(string userName, CancellationToken ct);
    bool CanHaveRoles(RoleName[] roles);
    Task<bool> CanHaveRolesAsync(RoleName[] roles, int id, CancellationToken ct);
}