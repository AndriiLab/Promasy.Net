using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Organizations.Interfaces;

internal interface ISubDepartmentsRules : IRepository
{
    Task<bool> IsExistAsync(int id, int departmentId, int organizationId, CancellationToken ct);
    Task<bool> IsNameUniqueAsync(string name, int departmentId, int organizationId, CancellationToken ct);
    Task<bool> IsNameUniqueAsync(string name, int id, int departmentId, int organizationId, CancellationToken ct);
    bool IsEditable(int id, int departmentId, int organizationId);
    Task<bool> IsUsedAsync(int id, CancellationToken ct);
}