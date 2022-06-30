using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApiDemo.Modules.Weather.Ports;

namespace MinimalApiDemo.Modules.Weather.Endpoints;

internal static class GetWeatherForecast
{
    public static async Task<IResult> Handler([FromQuery]int? days, IWeatherForecastRepository repository)
    {
        var fetchDays = days is > 0 and < 8 ? days.Value : 7;
        var weather = await repository.GetForecastsAsync(fetchDays);
        return Results.Ok(weather);
    }
}