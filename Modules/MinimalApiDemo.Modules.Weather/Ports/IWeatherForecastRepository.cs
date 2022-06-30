using MinimalApiDemo.Modules.Weather.Core;

namespace MinimalApiDemo.Modules.Weather.Ports;

internal interface IWeatherForecastRepository
{
    Task<WeatherForecast[]> GetForecastsAsync(int days);
}