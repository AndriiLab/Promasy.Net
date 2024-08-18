using Promasy.Application.Interfaces;
using Promasy.Domain.Suppliers;
using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Suppliers.Interfaces;

internal interface ISupplierRules : IRules<Supplier>
{
    Task<bool> IsNameUniqueAsync(string name, CancellationToken ct);
    Task<bool> IsNameUniqueAsync(string name, int id, CancellationToken ct);
    Task<bool> IsUsedAsync(int id, CancellationToken ct);
}