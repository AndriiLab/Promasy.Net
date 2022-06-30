using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiDemo.Modules.Core;
using MinimalApiDemo.Modules.Weather.Adapters;
using MinimalApiDemo.Modules.Weather.Core;
using MinimalApiDemo.Modules.Weather.Endpoints;
using MinimalApiDemo.Modules.Weather.Ports;

namespace MinimalApiDemo.Modules.Weather;

public class WeatherModule : IModule
{
    public IServiceCollection RegisterServices(IServiceCollection builder)
    {
        builder.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();

        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/weatherforecast", GetWeatherForecast.Handler)
            .WithName("GetWeatherForecast")
            .Produces<WeatherForecast[]>(200);

        return endpoints;
    }
}