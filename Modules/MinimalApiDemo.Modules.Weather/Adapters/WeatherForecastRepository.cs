using MinimalApiDemo.Modules.Weather.Core;
using MinimalApiDemo.Modules.Weather.Ports;

namespace MinimalApiDemo.Modules.Weather.Adapters;

internal class WeatherForecastRepository : IWeatherForecastRepository
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public Task<WeatherForecast[]> GetForecastsAsync(int days)
    {
        var forecast = Enumerable.Range(1, days).Select(index =>
                new WeatherForecast
                (
                    DateTime.UtcNow.AddDays(index),
                    Random.Shared.Next(-20, 55),
                    Summaries[Random.Shared.Next(Summaries.Length)]
                ))
            .ToArray();
        return Task.FromResult(forecast);
    }
}