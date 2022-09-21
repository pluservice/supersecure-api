using System.ComponentModel.DataAnnotations;

namespace SuperSecureAPI.DTO;

public record WeatherForecastRequest
{
    [Required]
    public string Place { get; set; } = "Ancona";

    [Required, Range(1, 7)]
    public int Days { get; set; } = 7;
}
