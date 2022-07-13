using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Core.Rules;

public interface IDepartmentsRules : IRules
{
    Task<bool> IsExistAsync(int id, CancellationToken ct);
    Task<bool> IsNameUniqueAsync(string name, CancellationToken ct);
    Task<bool> IsNameUniqueAsync(string name, int id, CancellationToken ct);
    bool IsEditable(int id);
    Task<bool> IsUsedAsync(int id, CancellationToken ct);
}