using Promasy.Core.Persistence;

namespace Promasy.Application.Interfaces;

public interface IRules<T> : IPermissionRules
    where T : IBaseEntity
{
    Task<bool> IsExistsAsync(int id, CancellationToken ct);
}

public interface IPermissionRules
{
    Task<bool> IsSameOrganizationAsync(int id, int userOrganizationId, CancellationToken ct);
    Task<bool> IsSameDepartmentAsync(int id, int userDepartmentId, CancellationToken ct);
    Task<bool> IsSameSubDepartmentAsync(int id, int userSubDepartmentId, CancellationToken ct);
    Task<bool> IsSameUserAsync(int id, int userId, CancellationToken ct);
}

public interface IPermissionRulesWithMultipleItems
{
    Task<bool> IsSameOrganizationAsync(int[] ids, int userOrganizationId, CancellationToken ct);
    Task<bool> IsSameDepartmentAsync(int[] ids, int userDepartmentId, CancellationToken ct);
    Task<bool> IsSameSubDepartmentAsync(int[] ids, int userSubDepartmentId, CancellationToken ct);
    Task<bool> IsSameUserAsync(int[] ids, int userId, CancellationToken ct);
}