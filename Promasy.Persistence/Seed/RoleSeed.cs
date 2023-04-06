using System;
using System.Linq;
using Promasy.Domain.Employees;
using Promasy.Persistence.Context;

namespace Promasy.Persistence.Seed;

internal static class RoleSeed
{
    public static void EnsureRolesCreated(PromasyContext context)
    {
        if (context.Roles.Count() == Enum.GetValues<RoleName>().Length)
        {
            return;
        }

        foreach (var role in Enum.GetValues<RoleName>().OrderBy(r => r))
        {
            EnsureRole(role, context);
        }
        
        context.SaveChanges();
    }
    
    private static void EnsureRole(RoleName role, PromasyContext context)
    {
        if (context.Roles.Any(r => r.Name == role))
        {
            return;
        }

        context.Roles.Add(new Role(role));
    }
}