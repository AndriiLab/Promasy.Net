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
    extension(RouteHandlerBuilder builder)
    {
        public RouteHandlerBuilder WithAuthorization(WebApplication app,
            string tag, string summary, PermissionAction action, params RoleName[] roleNames)
        {
            var permissionsService = app.Services.GetRequiredService<IPermissionsServiceBuilder>();
            var roles = roleNames.Length > 0 ? roleNames.Distinct().ToArray() : Enum.GetValues<RoleName>();
            foreach (var role in roles)
            {
                permissionsService.AddPermission(tag, action, role, PermissionCondition.Allowed);
            }

            builder.RequireAuthorization(p => UpdatePolicy(p, roles).Build());

            builder.WithApiDescription(tag, $"{tag}|{action}", summary);

            return builder;
        }

        public RouteHandlerBuilder WithAuthorizationAndValidation<TModel>(WebApplication app, 
            string tag, string summary, PermissionAction action, params (RoleName Role, PermissionCondition Condition)[] roleConditions)
            where TModel : IRequestWithPermissionValidation
        {
            var permissionsService = app.Services.GetRequiredService<IPermissionsServiceBuilder>();
            var list = roleConditions;
            if(list.Length < 1)
                list = Enum.GetValues<RoleName>().Select(r => (r, PermissionCondition.Allowed)).ToArray();
            
            foreach (var roleCondition in list.GroupBy(c => c.Role))
            {
                if (roleCondition.Count() > 1) 
                    throw new SecurityException($"More than one {nameof(PermissionCondition)} registered for {action}/{roleCondition.Key}: {string.Join(", ", roleCondition.Select(c => c.Item2))}");

                var rc = roleCondition.First();
                permissionsService.AddPermission(tag, action, rc.Role, rc.Condition);
            }

            builder.RequireAuthorization(p => UpdatePolicy(p, list.Select(rc => rc.Role).ToArray()).Build());
        
            builder.WithApiDescription(tag, $"{tag}|{action}", summary);

            builder.WithPermissionsValidator<TModel>(list);

            return builder;
        }
    }

    private static AuthorizationPolicyBuilder UpdatePolicy(AuthorizationPolicyBuilder apb,
        RoleName[] roles)
    {
        if (roles.Length > 0)
            apb = apb.RequireRole(roles.Select(rn => ((int)rn).ToString()));

        return apb;
    }
}