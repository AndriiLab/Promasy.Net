using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Core.Rules;

public interface IManufacturerRules : IRules
{
    Task<bool> IsNameUniqueAsync(string name, CancellationToken ct);
    Task<bool> IsNameUniqueAsync(string name, int id, CancellationToken ct);
    Task<bool> IsEditableAsync(int id, CancellationToken ct);
    Task<bool> IsUsedAsync(int id, CancellationToken ct);
}