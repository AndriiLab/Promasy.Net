using Promasy.Core.Persistence;

namespace Promasy.Modules.Core.Modules;

public interface IRules<T> where T : IBaseEntity
{
    Task<bool> IsExistsAsync(int id, CancellationToken ct);
}