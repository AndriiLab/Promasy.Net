using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Promasy.Domain.Users;
using Promasy.Persistence.Context;

namespace Promasy.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<PromasyContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }

        public static IApplicationBuilder PreparePersistence(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
            using var context = serviceScope.ServiceProvider.GetRequiredService<PromasyContext>();
            context.Database.Migrate();

            using var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            EnsureRolesCreated(context, roleManager);

            return app;
        }

        private static void EnsureRolesCreated(PromasyContext context, RoleManager<Role> roleManager)
        {
            EnsureRole(RoleName.Administrator, context, roleManager);
            EnsureRole(RoleName.Director, context, roleManager);
            EnsureRole(RoleName.DeputyDirector, context, roleManager);
            EnsureRole(RoleName.ChiefEconomist, context, roleManager);
            EnsureRole(RoleName.ChiefAccountant, context, roleManager);
            EnsureRole(RoleName.HeadOfTenderCommittee, context, roleManager);
            EnsureRole(RoleName.SecretaryOfTenderCommittee, context, roleManager);
            EnsureRole(RoleName.HeadOfDepartment, context, roleManager);
            EnsureRole(RoleName.PersonallyLiableEmployee, context, roleManager);
            EnsureRole(RoleName.User, context, roleManager);
        }

        private static void EnsureRole(string role, PromasyContext context, RoleManager<Role> roleManager)
        {
            if (context.Roles.Any(r => r.Name == role))
            {
                return;
            }

            roleManager.CreateAsync(new Role(role)).GetAwaiter().GetResult();
        }
    }
}