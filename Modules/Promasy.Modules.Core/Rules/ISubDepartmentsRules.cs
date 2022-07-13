using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Core.Rules;

public interface ISubDepartmentsRules : IRules
{
    Task<bool> IsExistAsync(int id, CancellationToken ct);
    Task<bool> IsNameUniqueAsync(string name, int departmentId, CancellationToken ct);
    Task<bool> IsNameUniqueAsync(string name, int id, int departmentId, CancellationToken ct);
    bool IsEditable(int id);
    Task<bool> IsUsedAsync(int id, CancellationToken ct);
}