using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Organizations.Interfaces;

internal interface IDepartmentsRules : IRepository
{
    Task<bool> IsExistAsync(int id, int organizationId, CancellationToken ct);
    Task<bool> IsNameUniqueAsync(string name, int organizationId, CancellationToken ct);
    Task<bool> IsNameUniqueAsync(string name, int id, int organizationId, CancellationToken ct);
    bool IsEditable(int id, int organizationId);
    Task<bool> IsUsedAsync(int id, CancellationToken ct);
}