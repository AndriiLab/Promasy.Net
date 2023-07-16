using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Promasy.Application.Interfaces;
using Promasy.Persistence.Context;
using Promasy.Persistence.Seed;
using Z.EntityFramework.Extensions;

namespace Promasy.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PromasyContext>(o =>
            {
                o.UseNpgsql(configuration.GetConnectionString("DatabaseConnection"));
                o.UseTriggers(c => c.AddAssemblyTriggers());
            });
            services.AddScoped<IDatabase, PromasyDatabase>();

            return services;
        }

        public static IApplicationBuilder PreparePersistence(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<PromasyContext>>();

            BatchDeleteManager.BatchDeleteBuilder = b =>
            {
                b.Executing = d => logger.LogInformation("SQL Batch Delete: {Sql}", d.CommandText);
            };            
            BatchUpdateManager.BatchUpdateBuilder = b =>
            {
                b.Executing = d => logger.LogInformation("SQL Batch Update: {Sql}", d.CommandText);
            };
            
            using var context = serviceScope.ServiceProvider.GetRequiredService<PromasyContext>();
            context.Database.Migrate();

            using var trx = context.Database.BeginTransaction();
            try
            {
                RoleSeed.EnsureRolesCreated(context);

                var seedingSettings = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>()
                    .GetSection("DefaultOrganizationSeed")
                    .Get<DefaultOrganizationSeedSettings>();

                OrganizationSeed.EnsureDefaultsCreated(context, seedingSettings);

                trx.Commit();
                
                return app;
            }
            catch (Exception)
            {
                trx.Rollback();
                throw;
            }
        }
    }
}