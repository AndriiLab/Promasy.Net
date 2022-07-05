using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Promasy.Domain.Employees;
using Promasy.Domain.Persistence;
using Promasy.Persistence.Context;

namespace Promasy.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<PromasyContext>(o =>
                o.UseNpgsql(connectionString));
            services.AddScoped<IDatabase, PromasyDatabase>();

            return services;
        }

        public static IApplicationBuilder PreparePersistence(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
            using var context = serviceScope.ServiceProvider.GetRequiredService<PromasyContext>();
            context.Database.Migrate();

            EnsureRolesCreated(context);
            context.SaveChanges();

            return app;
        }

        private static void EnsureRolesCreated(PromasyContext context)
        {
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
}