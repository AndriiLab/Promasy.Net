using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Promasy.Core.UserContext;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.UserContext;

namespace Promasy.Modules.Core;

public class CoreModule : IModule
{
    public string Tag { get; } = string.Empty;
    public string RoutePrefix { get; } = string.Empty;

    public IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        builder.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        builder.AddScoped<IUserContextResolver, UserContextResolver>();
        builder.AddScoped<IUserContext>(sp => sp.GetRequiredService<IUserContextResolver>().Resolve()!);
        
        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        return endpoints;
    }
}