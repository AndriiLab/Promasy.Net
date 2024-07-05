using System.Collections.Immutable;
using Promasy.Core.Exceptions;
using Promasy.Domain.Employees;

namespace Promasy.Modules.Core.Permissions;

public interface IPermissionsService
{
    ImmutableDictionary<string, ImmutableDictionary<RoleName, PermissionCondition>> GetAllPermissions();
    PermissionCondition? GetPermissionForRole(string tag, RoleName roleName);
}

internal class PermissionsService : IPermissionsService
{
    private readonly ImmutableDictionary<string, ImmutableDictionary<RoleName, PermissionCondition>> _dictionary;

    public PermissionsService(ImmutableDictionary<string, ImmutableDictionary<RoleName, PermissionCondition>> dictionary)
    {
        _dictionary = dictionary;
    }

    public ImmutableDictionary<string, ImmutableDictionary<RoleName, PermissionCondition>> GetAllPermissions()
        => _dictionary;
    
    public PermissionCondition? GetPermissionForRole(string tag, RoleName roleName)
    {
        if (_dictionary.TryGetValue(tag, out var dict) && dict.TryGetValue(roleName, out var condition))
            return condition;

        return null;
    }
}

internal interface IPermissionsServiceBuilder
{
    void AddPermission(string key, RoleName role, PermissionCondition condition);
    IPermissionsService Build();
}


internal class PermissionsServiceBuilder : IPermissionsServiceBuilder
{
    private readonly Dictionary<string, Dictionary<RoleName, PermissionCondition>> _dictionary;
    private bool _isBuilt;

    public PermissionsServiceBuilder()
    {
        _dictionary = new Dictionary<string, Dictionary<RoleName, PermissionCondition>>();
        _isBuilt = false;
    }

    public void AddPermission(string key, RoleName role, PermissionCondition condition)
    {
        if (_isBuilt)
            throw new ServiceException($"{nameof(IPermissionsService)} is already build. New entries cannot be added");

        if (!_dictionary.ContainsKey(key))
            _dictionary[key] = new Dictionary<RoleName, PermissionCondition>();

        if (_dictionary[key].TryGetValue(role, out var value))
            throw new ServiceException($"{key}/{role} has already defined value {value}");

        _dictionary[key][role] = condition;
    }

    public IPermissionsService Build()
    {
        _isBuilt = true;
        return new PermissionsService(_dictionary
            .ToImmutableDictionary(kv => kv.Key,
                kv => kv.Value.ToImmutableDictionary()));
    }
}