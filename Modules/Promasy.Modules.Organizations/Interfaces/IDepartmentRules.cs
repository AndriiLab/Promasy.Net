using Promasy.Domain.Organizations;
using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Organizations.Interfaces;

internal interface IDepartmentRules : IRules<Department>
{
    Task<bool> IsNameUniqueAsync(string name, CancellationToken ct);
    Task<bool> IsNameUniqueAsync(string name, int id, CancellationToken ct);
    bool IsEditable(int id);
    Task<bool> IsUsedAsync(int id, CancellationToken ct);
}