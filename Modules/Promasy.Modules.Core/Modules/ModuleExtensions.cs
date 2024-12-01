using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Promasy.Application.Interfaces;
using Promasy.Modules.Core.Validation;

namespace Promasy.Modules.Core.Modules;

public static class ModuleExtensions
{
    private static readonly ICollection<IModule> RegisteredModules = new List<IModule>();

    public static IServiceCollection RegisterModule<TModule>(this IServiceCollection services, IConfiguration configuration) where TModule : class, IModule
    {
        // register assembly validators
        services.AddValidatorsFromAssembly(typeof(TModule).Assembly, includeInternalTypes: true);

        // register module services decorated with IService and repositories decorated with IRepository
        // and rules decorated with IRules
        services.Scan(scan => scan
            .FromAssemblyOf<TModule>()
            .AddClasses(classes => classes.AssignableTo<IService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<IRepository>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IRules<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IPermissionsValidator<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        // register additional module services
        var module = Activator.CreateInstance<TModule>();
        module.RegisterServices(services, configuration);
        
        RegisteredModules.Add(module);

        return services;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        foreach (var module in RegisteredModules)
        {
            module.MapEndpoints(app);
        }
        return app;
    }
}