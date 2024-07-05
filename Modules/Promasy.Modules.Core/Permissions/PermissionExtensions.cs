using System.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Promasy.Domain.Employees;
using Promasy.Modules.Core.OpenApi;
using Promasy.Modules.Core.Validation;

namespace Promasy.Modules.Core.Permissions;

public static class PermissionExtensions
{
    public static RouteHandlerBuilder WithAuthorization(this RouteHandlerBuilder builder, WebApplication app,
        string tag, string summary, Func<string, PermissionTag> idGen, params RoleName[] roleNames)
    {
        var permissionsService = app.Services.GetRequiredService<IPermissionsServiceBuilder>();
        var roles = roleNames.Distinct().ToArray();
        var id = idGen(tag).Name;
        foreach (var role in roles)
        {
            permissionsService.AddPermission(id, role, PermissionCondition.None);
        }

        builder.RequireAuthorization(p => UpdatePolicy(p, roles).Build());

        builder.WithApiDescription(tag, id, summary);

        return builder;
    }
    
    public static RouteHandlerBuilder WithAuthorizationAndValidation<TModel>(this RouteHandlerBuilder builder, WebApplication app, 
        string tag, string summary, Func<string, PermissionTag> idGen, params (RoleName Role, PermissionCondition Condition)[] roleConditions)
        where TModel : class
    {
        var permissionsService = app.Services.GetRequiredService<IPermissionsServiceBuilder>();
            
        var id = idGen(tag).Name;
        foreach (var roleCondition in roleConditions.GroupBy(c => c.Role))
        {
            if (roleCondition.Count() > 1)
                throw new SecurityException($"More than one {nameof(PermissionCondition)} registered for {id}/{roleCondition.Key}: {string.Join(", ", roleCondition.Select(c => c.Item2))}");

            var rc = roleCondition.First();
            permissionsService.AddPermission(id, rc.Role, rc.Condition);
        }

        builder.RequireAuthorization(p => UpdatePolicy(p, roleConditions.Select(rc => rc.Role).ToArray()).Build());
        
        builder.WithApiDescription(tag, id, summary);

        builder.WithValidator<TModel>(); // todo: pass roleConditions into validation

        return builder;
    }

    private static AuthorizationPolicyBuilder UpdatePolicy(AuthorizationPolicyBuilder apb,
        RoleName[] roles)
    {
        if (roles.Length > 0)
            apb = apb.RequireRole(roles.Select(rn => ((int)rn).ToString()));

        return apb;
    }
}

public class PermissionTag
{
    public string Name { get; }
    private PermissionTag(string name)
    {
        Name = name;
    }
    
    public static PermissionTag List(string endpoint) => new($"List/{endpoint}");
    public static PermissionTag Get(string endpoint) => new($"Get/{endpoint}");
    public static PermissionTag Create(string endpoint) => new($"Create/{endpoint}");
    public static PermissionTag Update(string endpoint) => new($"Update/{endpoint}");
    public static PermissionTag Delete(string endpoint) => new($"Delete/{endpoint}");
    public static PermissionTag Merge(string endpoint) => new($"Merge/{endpoint}");
    public static PermissionTag Export(string endpoint) => new($"Export/{endpoint}");
}