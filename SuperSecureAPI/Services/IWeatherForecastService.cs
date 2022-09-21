using System.ComponentModel.DataAnnotations;

namespace SuperSecureAPI.Services;

public interface IWeatherForecastService
{
    public Task<IEnumerable<WeatherForecast>> GetForecasts1(WeatherForecastRequest request);
    public Task<IEnumerable<WeatherForecast>> GetForecasts2(WeatherForecastRequest request);
    public Task<IEnumerable<WeatherForecast>> GetForecasts3(WeatherForecastRequest request);
    public Task<IEnumerable<WeatherForecast>> GetForecasts4(WeatherForecastRequest request);
}

public class WeatherForecast
{
    public string? Place { get; set; }

    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}

public record WeatherForecastRequest
{
    [Required]
    public string Place { get; set; } = string.Empty;

    [Required, Range(1, 7)]
    public int Days { get; set; } = 7;
}
