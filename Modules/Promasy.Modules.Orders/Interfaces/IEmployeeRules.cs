using Promasy.Domain.Employees;

namespace Promasy.Modules.Orders.Interfaces;

internal interface IEmployeeRules
{
    Task<bool> IsExistsWithRoleAsync(int id, RoleName role, CancellationToken ct);
}