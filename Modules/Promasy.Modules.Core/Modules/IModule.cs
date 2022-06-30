using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Promasy.Modules.Core.Modules;

public interface IModule
{
    string Tag { get; }
    string RoutePrefix { get; }
    IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration);
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}