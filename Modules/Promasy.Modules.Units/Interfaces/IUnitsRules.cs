using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Units.Interfaces;

internal interface IUnitsRules : IRepository
{
    Task<bool> IsExistAsync(int id, CancellationToken ct);
    Task<bool> IsNameUniqueAsync(string name, CancellationToken ct);
    Task<bool> IsNameUniqueAsync(string name, int id, CancellationToken ct);
    Task<bool> IsUnitUsedAsync(int id, CancellationToken ct);
}