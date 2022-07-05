using Microsoft.AspNetCore.Authorization;

namespace Promasy.Modules.Core.Policies;

public interface IAuthorizationPolicy
{
    static string Name { get; }

    AuthorizationPolicy GetPolicy();
}