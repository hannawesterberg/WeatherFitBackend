

using System;
using Newtonsoft.Json;
namespace WeatherFit.Entities
{
    // This is your existing WeatherData class, which should be defined according to the OpenWeatherMap's weather response JSON
    // Properties that match the JSON response from the OpenWeatherMap API

    public class WeatherData
    {
        public string City { get; set; } // Add city name to the weather data model
        public Coord? Coord { get; set; } 

        public List<Weather> Weather { get; set; }

        public Main Main { get; set; }

        public Wind Wind { get; set; }

        public Clouds Clouds { get; set; }

    }

    //Coordinates
    public class Coord
    {
        [JsonProperty("lon")]
        public double Longitude { get; set; }
        [JsonProperty("lat")]
        public double Latitude { get; set; }
    }

    //Weather
    public class Weather
    {
        [JsonProperty("main")]
        public string ShortDescription { get; set; }
        [JsonProperty("description")]
        public string LongDescription { get; set; }

    }

    //Main
    public class Main
    {
        [JsonProperty("temp")] //kelvin scale 
        public double Temp { get; set; }
        [JsonProperty("feels_like")] //kelvin scale 
        public double Feels_Like { get; set; }
        //[JsonProperty("humidity")]
        //public int Humidity { get; set; }
    }

    //Wind
    public class Wind
    {
        [JsonProperty("speed")] //Windspeed, meter/sec 
        public double WindSpeed { get; set; }
    }

    //Cloud
    public class Clouds
    {
        [JsonProperty("all")] //cloudiness in %
        public int Cloudiness { get; set; }
    }

}


/*
using System;
using Newtonsoft.Json;
namespace WeatherFit.Entities
{
    // This is your existing WeatherData class, which should be defined according to the OpenWeatherMap's weather response JSON
    // Properties that match the JSON response from the OpenWeatherMap API

    public class WeatherData
    {
        public Coord Coord { get; set; } 

        public List<ShortWeather> ShortWeather { get; set; }

        public LongWeather LongWeather { get; set; }

        public Main Main { get; set; }

        public Wind Wind { get; set; }

        public Cloud Cloud { get; set; }

        // public string City { get; set; } create 

    }

    //Coordinates
    public class Coord
    {
        [JsonProperty("lon")]
        public double Longitude { get; set; }
        [JsonProperty("lat")]
        public double Latitude { get; set; }
    }

    //Weather
    public class ShortWeather
    {
        [JsonProperty("main")]
        public string ShortDescription { get; set; }

    }

    public class LongWeather
    {

        [JsonProperty("description")]
        public string LongDescription { get; set; }

    }


    //Main
    public class Main
    {
        [JsonProperty("temp")] //kelvin scale 
        public double Temp { get; set; }
        [JsonProperty("feels_like")] //kelvin scale 
        public double Feels_Like { get; set; }
        //[JsonProperty("humidity")]
        //public int Humidity { get; set; }
    }

    //Wind
    public class Wind
    {
        [JsonProperty("speed")] //Windspeed, meter/sec 
        public double WindSpeed { get; set; }
    }

    //Cloud
    public class Cloud
    {
        [JsonProperty("all")] //cloudiness in %
        public int Cloudiness { get; set; } //CHECK CLOUD VARIABLE
    }

}
*/