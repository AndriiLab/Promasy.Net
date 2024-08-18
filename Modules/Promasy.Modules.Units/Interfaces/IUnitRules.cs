using Promasy.Application.Interfaces;
using Promasy.Domain.Orders;
using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Units.Interfaces;

internal interface IUnitRules : IRules<Unit>
{
    Task<bool> IsNameUniqueAsync(string name, CancellationToken ct);
    Task<bool> IsNameUniqueAsync(string name, int id, CancellationToken ct);
    Task<bool> IsUsedAsync(int id, CancellationToken ct);
}