using AutoMapper;

namespace SuperSecureAPI.Services;

public class WeatherForecastMappingProfile : Profile
{
    public WeatherForecastMappingProfile()
    {
        CreateMap<DTO.WeatherForecastRequest, WeatherForecastRequest>();
        CreateMap<WeatherForecast, DTO.WeatherForecast>();
    }
}
