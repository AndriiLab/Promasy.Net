using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Serialization;

namespace Promasy.Modules.Core;

public class CoreModule : IModule
{
    public string Tag { get; } = "";
    public string RoutePrefix { get; } = "";

    public IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        builder.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new DateOnlyConverter()));
        builder.AddSingleton<IPermissionsServiceBuilder, PermissionsServiceBuilder>();
        builder.AddSingleton<IPermissionsService>(s => s.GetRequiredService<IPermissionsServiceBuilder>().Build());

        return builder;
    }

    public WebApplication MapEndpoints(WebApplication app)
    {
        return app;
    }
}