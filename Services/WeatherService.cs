using Newtonsoft.Json;
using WeatherFit.Entities;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "d234084d052e88b8f35839c47497c73f";

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private class LocationResponse
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
     
    }

    //Method to convert city to lat & long using a geocoding web api 
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

    // GetWeatherAsync method to get weather data from the city 
    public async Task<WeatherData> GetWeatherAsync(string city)
    {
        var (Latitude, Longitude) = await GetCoordinatesAsync(city);
        string weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={Latitude}&lon={Longitude}&appid={_apiKey}";
        HttpResponseMessage weatherResponse = await _httpClient.GetAsync(weatherUrl);
        weatherResponse.EnsureSuccessStatusCode();
        var weatherData = JsonConvert.DeserializeObject<WeatherData>(await weatherResponse.Content.ReadAsStringAsync());
        weatherData.City = city;  
        return weatherData;
       
    }
    
}
