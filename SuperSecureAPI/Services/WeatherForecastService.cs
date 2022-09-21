using System.Data.SqlClient;
using Dapper;

namespace SuperSecureAPI.Services;

/// <summary>
/// These implementations all contain a VERY USEFUL query, whose sole purpose is
/// to feature a SQL Injection vulnerability in order to test SASTs performance
/// </summary>
public class WeatherForecastService : IWeatherForecastService
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    private readonly IConfiguration configuration;

    public WeatherForecastService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<IEnumerable<WeatherForecast>> GetForecasts1(WeatherForecastRequest request)
    {
        string? connectionString = configuration.GetConnectionString("WeatherDb");
        using SqlConnection conn = new SqlConnection(connectionString);

        using SqlCommand sqlCommand = conn.CreateCommand();
        sqlCommand.CommandType = System.Data.CommandType.Text;
        // SQL INJECTION VULN (DETECTED AS SECURITY HOTSPOT BUT NOT AS VULNERABILITY)
        sqlCommand.CommandText = "SELECT '" + request.Place + "'";
        await conn.OpenAsync();
        using SqlDataReader reader = sqlCommand.ExecuteReader();
        string placeFromDb = (await reader.ReadAsync()) ? reader.GetString(0) : request.Place;

        WeatherForecast[] forecasts = GenerateRandomForecasts(placeFromDb ?? request.Place, request.Days);
        return forecasts;
    }

    public async Task<IEnumerable<WeatherForecast>> GetForecasts2(WeatherForecastRequest request)
    {
        string? connectionString = configuration.GetConnectionString("WeatherDb");
        using SqlConnection conn = new SqlConnection(connectionString);

        using SqlCommand sqlCommand = conn.CreateCommand();
        sqlCommand.CommandType = System.Data.CommandType.Text;
        // SQL INJECTION VULN (DETECTED AS SECURITY HOTSPOT BUT NOT AS VULNERABILITY)
        sqlCommand.CommandText = $"SELECT '{request.Place}'";
        await conn.OpenAsync();
        using SqlDataReader reader = sqlCommand.ExecuteReader();
        string placeFromDb = (await reader.ReadAsync()) ? reader.GetString(0) : request.Place;

        WeatherForecast[] forecasts = GenerateRandomForecasts(placeFromDb ?? request.Place, request.Days);
        return forecasts;
    }

    public async Task<IEnumerable<WeatherForecast>> GetForecasts3(WeatherForecastRequest request)
    {
        string? connectionString = configuration.GetConnectionString("WeatherDb");
        using SqlConnection conn = new SqlConnection(connectionString);

        using SqlCommand sqlCommand = conn.CreateCommand();
        sqlCommand.CommandType = System.Data.CommandType.Text;
        // SQL INJECTION VULN (DETECTED AS SECURITY HOTSPOT BUT NOT AS VULNERABILITY)
        sqlCommand.CommandText = string.Format("SELECT '{0}'", request.Place);
        await conn.OpenAsync();
        using SqlDataReader reader = sqlCommand.ExecuteReader();
        string placeFromDb = (await reader.ReadAsync()) ? reader.GetString(0) : request.Place;

        WeatherForecast[] forecasts = GenerateRandomForecasts(placeFromDb ?? request.Place, request.Days);
        return forecasts;
    }

    public async Task<IEnumerable<WeatherForecast>> GetForecasts4(WeatherForecastRequest request)
    {
        string? connectionString = configuration.GetConnectionString("WeatherDb");
        using SqlConnection conn = new SqlConnection(connectionString);

        // SQL INJECTION VULN (NOT DETECTED)
        string placeFromDb = await conn.QueryFirstOrDefaultAsync<string>($"SELECT '{request.Place}'") ?? request.Place;

        WeatherForecast[] forecasts = GenerateRandomForecasts(placeFromDb ?? request.Place, request.Days);
        return forecasts;
    }

    private static WeatherForecast[] GenerateRandomForecasts(string placeName, int numberOfDays)
    {
        return Enumerable
            .Range(1, numberOfDays)
            .Select(index => new WeatherForecast
            {
                Place = placeName,
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
}
