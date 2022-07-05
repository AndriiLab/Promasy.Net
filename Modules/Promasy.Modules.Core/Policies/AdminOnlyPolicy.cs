using Microsoft.AspNetCore.Authorization;
using Promasy.Domain.Employees;

namespace Promasy.Modules.Core.Policies;

public class AdminOnlyPolicy : IAuthorizationPolicy
{
    public const string Name = "AdminOnly";
    
    public AuthorizationPolicy GetPolicy()
    {
        return new AuthorizationPolicyBuilder()
            .RequireRole(((int) RoleName.Administrator).ToString())
            .Build();
    }
}