using Microsoft.EntityFrameworkCore;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;

namespace Promasy.Application.Helpers;

public static class PermissionRulesRepositoryHelper
{
    public static Task<bool> IsSameOrganizationAsync<TEntity>(IDatabase database, int id, int userOrganizationId,
        CancellationToken ct)
        where TEntity : class, IBaseEntity, IOrganizationAssociated
    {
        return GetQueryable<TEntity>(database)
            .AnyAsync(o => o.Id == id && o.OrganizationId == userOrganizationId, ct);
    }
    
    public static Task<bool> IsSameOrganizationAsync<TEntity>(IDatabase database, int[] ids, int userOrganizationId,
        CancellationToken ct)
        where TEntity : class, IBaseEntity, IOrganizationAssociated
    {
        return GetQueryable<TEntity>(database)
            .Where(o => ids.Contains(o.Id))
            .AllAsync(o => o.OrganizationId == userOrganizationId, ct);
    }

    public static Task<bool> IsSameDepartmentAsync<TEntity>(IDatabase database, int id, int userDepartmentId,
        CancellationToken ct)
        where TEntity : class, IBaseEntity, IOrganizationAssociated
    {
        var employees = database.Departments.AsNoTracking()
            .Where(s => s.Id == userDepartmentId)
            .SelectMany(s => s.SubDepartments)
            .SelectMany(s => s.Employees).Select(e => e.Id);
        return GetQueryable<TEntity>(database)
            .AnyAsync(
                o => o.Id == id && (employees.Contains(o.CreatorId) ||
                                    o.ModifierId.HasValue && employees.Contains(o.ModifierId.Value)), ct);
    }

    public static Task<bool> IsSameSubDepartmentAsync<TEntity>(IDatabase database, int id, int userSubDepartmentId,
        CancellationToken ct)
        where TEntity : class, IBaseEntity, IOrganizationAssociated
    {
        var employees = database.SubDepartments.AsNoTracking()
            .Where(s => s.Id == userSubDepartmentId)
            .SelectMany(s => s.Employees).Select(e => e.Id);
        return GetQueryable<TEntity>(database)
            .AnyAsync(
                o => o.Id == id && (employees.Contains(o.CreatorId) ||
                                    o.ModifierId.HasValue && employees.Contains(o.ModifierId.Value)), ct);
    }

    public static Task<bool> IsSameUserAsync<TEntity>(IDatabase database, int id, int userId, CancellationToken ct)
        where TEntity : class, IBaseEntity
    {
        return GetQueryable<TEntity>(database)
            .AnyAsync(o => o.Id == id && (o.ModifierId == userId || o.CreatorId == userId), ct);
    }
    
    public static Task<bool> IsSameUserAsync<TEntity>(IDatabase database, int[] ids, int userId, CancellationToken ct)
        where TEntity : class, IBaseEntity
    {
        return GetQueryable<TEntity>(database)
            .Where(o => ids.Contains(o.Id))
            .AllAsync(o => o.ModifierId == userId || o.CreatorId == userId, ct);
    }

    private static IQueryable<TEntity> GetQueryable<TEntity>(IDatabase database)
        where TEntity : class, IBaseEntity
    {
        return ((IExtendedDatabase)database).GetDbSet<TEntity>().AsNoTracking();
    }
}