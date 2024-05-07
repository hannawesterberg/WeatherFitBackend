using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WeatherFit;
using WeatherFit;

namespace WeatherFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        //constructor initiating _weatherservice

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        //GET method to retrieve city
        [HttpGet("{city}")]
        public async Task<IActionResult> GetWeatherByCity(string city)
        {
            try
            {
                var weatherData = await _weatherService.GetWeatherAsync(city);
                return Ok(weatherData);
            }
            catch (HttpRequestException ex)
            {
                return NotFound(ex.Message);
            }
        }

    }


}

