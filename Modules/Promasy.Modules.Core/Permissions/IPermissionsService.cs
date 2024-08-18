using System.Collections.Immutable;
using Promasy.Core.Exceptions;
using Promasy.Domain.Employees;
using Promasy.Modules.Core.Responses;

namespace Promasy.Modules.Core.Permissions;

public interface IPermissionsService
{
    IReadOnlyCollection<EndpointPermission> GetPermissionForRoles(IReadOnlyCollection<RoleName> roleNames);
}

internal class PermissionsService : IPermissionsService
{
    private readonly ImmutableDictionary<string, ImmutableDictionary<RoleName, PermissionCondition>> _dictionary;

    public PermissionsService(ImmutableDictionary<string, ImmutableDictionary<RoleName, PermissionCondition>> dictionary)
    {
        _dictionary = dictionary;
    }
    
    public IReadOnlyCollection<EndpointPermission> GetPermissionForRoles(IReadOnlyCollection<RoleName> roleNames)
    {
        return _dictionary.Select(kv =>
                new EndpointPermission(kv.Key, kv.Value.Where(kv2 => roleNames.Contains(kv2.Key)).Max(kv2 => kv2.Value)))
            .Where(t => t.Condition > PermissionCondition.Forbidden)
            .ToImmutableArray();

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