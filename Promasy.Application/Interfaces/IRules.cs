using Promasy.Core.Persistence;

namespace Promasy.Application.Interfaces;

public interface IRules<T> where T : IBaseEntity
{
    Task<bool> IsExistsAsync(int id, CancellationToken ct);
}