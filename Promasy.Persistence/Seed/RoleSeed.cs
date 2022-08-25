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

        EnsureRole(RoleName.Administrator, context);
        EnsureRole(RoleName.Director, context);
        EnsureRole(RoleName.DeputyDirector, context);
        EnsureRole(RoleName.ChiefEconomist, context);
        EnsureRole(RoleName.ChiefAccountant, context);
        EnsureRole(RoleName.HeadOfTenderCommittee, context);
        EnsureRole(RoleName.SecretaryOfTenderCommittee, context);
        EnsureRole(RoleName.HeadOfDepartment, context);
        EnsureRole(RoleName.PersonallyLiableEmployee, context);
        EnsureRole(RoleName.User, context);
        
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