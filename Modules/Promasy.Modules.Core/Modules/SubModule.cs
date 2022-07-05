using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Promasy.Modules.Core.Modules;

public abstract class SubModule : IModule
{
    public abstract string Tag { get; }
    public string RoutePrefix { get; }

    public SubModule(string parentRoutePrefix, string submodulePrefix)
    {
        RoutePrefix = $"{parentRoutePrefix}{submodulePrefix}";
    }
    public abstract IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration);

    public abstract IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}