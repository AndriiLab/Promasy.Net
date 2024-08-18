using Promasy.Application.Interfaces;
using Promasy.Domain.Manufacturers;

namespace Promasy.Modules.Manufacturers.Interfaces;

internal interface IManufacturerRules : IRules<Manufacturer>, IPermissionRules
{
    Task<bool> IsNameUniqueAsync(string name, CancellationToken ct);
    Task<bool> IsNameUniqueAsync(string name, int id, CancellationToken ct);
    Task<bool> IsUsedAsync(int id, CancellationToken ct);
}