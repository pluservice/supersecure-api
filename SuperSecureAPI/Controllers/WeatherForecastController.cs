using System.Data.SqlClient;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SuperSecureAPI.Services;

namespace SuperSecureAPI.Controllers;

/// <summary>
/// Weather forecasts controller.
/// All the methods are subject to SQL Injection attacks
/// </summary>
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IWeatherForecastService forecastService;
    private readonly IMapper mapper;

    public WeatherForecastController(IConfiguration configuration, IWeatherForecastService forecastService, IMapper mapper)
    {
        this.configuration = configuration;
        this.forecastService = forecastService;
        this.mapper = mapper;
    }

    /// <summary>
    /// This implementation directly contains code with a SQL Injection vuln
    /// </summary>
    /// <remarks>
    /// To test the vulnerability, try to pass the string
    /// <code>Roma' as town into #dump; select 'injected</code>
    /// in the "place" parameter of the request body.
    /// The other implementations are more similar to real cases, since they use
    /// a service which contains the business logic, but they appear to be
    /// harder to detect
    /// </remarks>
    [HttpPost("Implementation0")]
    [ProducesResponseType(typeof(IEnumerable<DTO.WeatherForecast>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Implementation0([FromBody] DTO.WeatherForecastRequest request)
    {
        string? connectionString = configuration.GetConnectionString("WeatherDb");
        using SqlConnection conn = new SqlConnection(connectionString);

        using SqlCommand sqlCommand = conn.CreateCommand();
        sqlCommand.CommandType = System.Data.CommandType.Text;
        // SQL INJECTION VULN (DETECTED AS BOTH VULNERABILITY AND SECURITY HOTSPOT)
        sqlCommand.CommandText = "SELECT '" + request.Place + "'";
        await conn.OpenAsync();
        using SqlDataReader reader = sqlCommand.ExecuteReader();
        string placeFromDb = (await reader.ReadAsync()) ? reader.GetString(0) : request.Place;

        string[] summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        WeatherForecast[] forecasts = Enumerable
            .Range(1, request.Days)
            .Select(index => new WeatherForecast
            {
                Place = placeFromDb ?? request.Place,
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            })
            .ToArray();

        return Ok(forecasts);
    }

    /// <summary>
    /// Endpoint using one of the <see cref="WeatherForecastService"/>
    /// implementations (which contains a SQL Injection vuln)
    /// </summary>
    /// <remarks>
    /// To test the vulnerability, try to pass the string
    /// <code>Roma' as town into #dump; select 'injected</code>
    /// in the "place" parameter of the request body.
    /// </remarks>
    [HttpPost("Implementation1")]
    [ProducesResponseType(typeof(IEnumerable<DTO.WeatherForecast>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Implementation1([FromBody] DTO.WeatherForecastRequest request)
    {
        IEnumerable<WeatherForecast> forecasts = await forecastService.GetForecasts1(mapper.Map<WeatherForecastRequest>(request));
        return ForecastsActionResult(forecasts);
    }

    /// <summary>
    /// Endpoint using one of the <see cref="WeatherForecastService"/>
    /// implementations (which contains a SQL Injection vuln)
    /// </summary>
    /// <remarks>
    /// To test the vulnerability, try to pass the string
    /// <code>Roma' as town into #dump; select 'injected</code>
    /// in the "place" parameter of the request body.
    /// </remarks>
    [HttpPost("Implementation2")]
    [ProducesResponseType(typeof(IEnumerable<DTO.WeatherForecast>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Implementation2([FromBody] DTO.WeatherForecastRequest request)
    {
        IEnumerable<WeatherForecast> forecasts = await forecastService.GetForecasts2(mapper.Map<WeatherForecastRequest>(request));
        return ForecastsActionResult(forecasts);
    }

    /// <summary>
    /// Endpoint using one of the <see cref="WeatherForecastService"/>
    /// implementations (which contains a SQL Injection vuln)
    /// </summary>
    /// <remarks>
    /// To test the vulnerability, try to pass the string
    /// <code>Roma' as town into #dump; select 'injected</code>
    /// in the "place" parameter of the request body.
    /// </remarks>
    [HttpPost("Implementation3")]
    [ProducesResponseType(typeof(IEnumerable<DTO.WeatherForecast>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Implementation3([FromBody] DTO.WeatherForecastRequest request)
    {
        IEnumerable<WeatherForecast> forecasts = await forecastService.GetForecasts3(mapper.Map<WeatherForecastRequest>(request));
        return ForecastsActionResult(forecasts);
    }

    /// <summary>
    /// Endpoint using one of the <see cref="WeatherForecastService"/>
    /// implementations (which contains a SQL Injection vuln)
    /// </summary>
    /// <remarks>
    /// To test the vulnerability, try to pass the string
    /// <code>Roma' as town into #dump; select 'injected</code>
    /// in the "place" parameter of the request body.
    /// </remarks>
    [HttpPost("Implementation4")]
    [ProducesResponseType(typeof(IEnumerable<DTO.WeatherForecast>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Implementation4([FromBody] DTO.WeatherForecastRequest request)
    {
        IEnumerable<WeatherForecast> forecasts = await forecastService.GetForecasts4(mapper.Map<WeatherForecastRequest>(request));
        return ForecastsActionResult(forecasts);
    }


    [HttpPost("Implementation5")]
    [ProducesResponseType(typeof(IEnumerable<DTO.WeatherForecast>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Implementation5([FromBody] DTO.WeatherForecastRequest request)
    {
        var v = new { Amount = 108, Message = "Hello" };
        var vjson = JsonConvert.SerializeObject(v);

        IEnumerable<WeatherForecast> forecasts = await forecastService.GetForecasts4(mapper.Map<WeatherForecastRequest>(request));
        return ForecastsActionResult(forecasts);
    }


    /// <summary>
    /// Helper method to build responses
    /// </summary>
    private IActionResult ForecastsActionResult(IEnumerable<WeatherForecast> forecasts)
    {
        if (forecasts == null) return NotFound();
        if (!forecasts.Any()) return NotFound();
        return Ok(mapper.Map<IEnumerable<DTO.WeatherForecast>>(forecasts));
    }
}
