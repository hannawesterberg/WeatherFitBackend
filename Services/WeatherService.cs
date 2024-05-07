using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using WeatherFit.Entities;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "d234084d052e88b8f35839c47497c73f";

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // New method to get latitude and longitude
    private async Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string city)
    {
        string geocodingUrl = $"http://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={_apiKey}";
        HttpResponseMessage geoResponse = await _httpClient.GetAsync(geocodingUrl);
        geoResponse.EnsureSuccessStatusCode();
        string geoResponseBody = await geoResponse.Content.ReadAsStringAsync();

        var locationList = JsonConvert.DeserializeObject<List<LocationResponse>>(geoResponseBody);
        var location = locationList.FirstOrDefault();

        if (location != null)
        {
            return (location.Lat, location.Lon);
        }
        throw new HttpRequestException("Geocoding failed, city not found");
    }

    // Modified GetWeatherAsync method to use coordinates
    public async Task<WeatherData> GetWeatherAsync(string city)
    {
        var (Latitude, Longitude) = await GetCoordinatesAsync(city);
        string weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={Latitude}&lon={Longitude}&appid={_apiKey}";
        HttpResponseMessage weatherResponse = await _httpClient.GetAsync(weatherUrl);
        weatherResponse.EnsureSuccessStatusCode();
        var weatherData = JsonConvert.DeserializeObject<WeatherData>(await weatherResponse.Content.ReadAsStringAsync());
        weatherData.City = city;  // Set the city name in your weather data
        return weatherData;


        /*
        var (Latitude, Longitude) = await GetCoordinatesAsync(city);
        string weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={Latitude}&lon={Longitude}&appid={_apiKey}";
        HttpResponseMessage weatherResponse = await _httpClient.GetAsync(weatherUrl);
        weatherResponse.EnsureSuccessStatusCode();
        string weatherResponseBody = await weatherResponse.Content.ReadAsStringAsync();
        weatherData.City = city;  // Set the city name in your weather data
        return JsonConvert.DeserializeObject<WeatherData>(weatherResponseBody);
        */
       
    }

    // The LocationResponse class matches the JSON structure returned by the Geocoding API
    private class LocationResponse
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        // ... include other fields as necessary
    }
}
