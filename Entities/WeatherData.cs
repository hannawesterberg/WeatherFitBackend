using Newtonsoft.Json; //Converting Json to .NET 

namespace WeatherFit.Entities
{
    public class WeatherData
    {
        public string City { get; set; } 
        public Coord? Coord { get; set; } 

        public List<Weather> Weather { get; set; }

        public Main Main { get; set; }

        public Wind Wind { get; set; }

        public Clouds Clouds { get; set; }

    }

    public class Coord
    {
        [JsonProperty("lon")]
        public double Longitude { get; set; }
        [JsonProperty("lat")]
        public double Latitude { get; set; }
    }

    public class Weather
    {
        [JsonProperty("main")]
        public string ShortDescription { get; set; }
        [JsonProperty("description")]
        public string LongDescription { get; set; }

    }

    public class Main
    {
        [JsonProperty("temp")] //Measured in kelvin scale 
        public double Temp { get; set; }
        [JsonProperty("feels_like")] //Measured in kelvin scale 
        public double Feels_Like { get; set; }
    }

    public class Wind
    {
        [JsonProperty("speed")] //Measured in m/sec 
        public double WindSpeed { get; set; }
    }

    public class Clouds
    {
        [JsonProperty("all")] //Measured in %
        public int Cloudiness { get; set; }
    }

}