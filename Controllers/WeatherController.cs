using Microsoft.AspNetCore.Mvc; //To access controllerbase to handle request and response 

namespace WeatherFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService; 

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        //GET: /api/Weather/{city} -  method to get the current weather in a city
        [HttpGet("{city}")]
        public async Task<IActionResult> GetWeatherByCity(string city)
        {
            try
            {
                var weatherData = await _weatherService.GetWeatherAsync(city); //Calling on the method from Weather Service 
                return Ok(weatherData);
            }
            catch (HttpRequestException ex)
            {
                return NotFound(ex.Message);
            }
        }

    }

}

