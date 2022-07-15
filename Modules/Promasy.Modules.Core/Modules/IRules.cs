namespace Promasy.Modules.Core.Modules;

public interface IRules
{
    Task<bool> IsExistsAsync(int id, CancellationToken ct);
}