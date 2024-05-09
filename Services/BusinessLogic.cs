using Microsoft.EntityFrameworkCore;
using WeatherFit.Data;
using WeatherFit.Entities;

namespace WeatherFit.Services
{

    public class Recommendation 
    {
        public string? ReturnMessage { get; set; } 
    }
    public class BusinessLogic
    {
        private readonly ApplicationDbContext _context;

        public BusinessLogic(ApplicationDbContext context)
        {
            _context = context;
        }


        public Recommendation GetRecommendation(WeatherData weather, Preferences preference, Users users, WeatherService ws)
        {

            var recommendation = new Recommendation(); // recommendation output 
            string outfitSuggestion = "";               // Var for ehich outfit suggestion will be sidplayed based on 5 intervalls 
            double feelsLikeCelsius = weather.Main?.Feels_Like - 273.15 ?? 0; // Converting Kelvin to Celsius 
            string formattedTemperature = feelsLikeCelsius.ToString("0"); //Formatting the celsius to no decimals 

            // Ensuring weather conditions are not null before processing
            if (weather.Main != null && weather.Weather != null && weather.Wind != null)
            {
                //SUNGLASS RECOMMENDATION
                var sunglasses = "";
                if (weather.Clouds != null)
                {
                    sunglasses =weather.Clouds.Cloudiness <=20
                         ? " Also, since the sky is clear you might want to wear a pair of sunglasses."
                                 : "";

                }
                                
                //CREATING VARIABLES FOR THE SUGGESTIONS 
                double tempCelsius = weather.Main.Temp - 273.15; //Creating a new variable for tempCelsius 
                bool isRaining = weather.Weather.Any(w => w.ShortDescription.Contains("Rain")); //Creating variable for raining 


                //INITIALIZE METHOD GetOutfitSuggestion() and set the return to outfitSuggestion 
                outfitSuggestion = GetOutfitSuggestion(tempCelsius, isRaining, weather.Wind.WindSpeed, preference);

                recommendation.ReturnMessage = $"Hi {users.Name}, for your {preference.Activity} in {weather.City}, " + 
                                               $"here is our recommendation for what to wear: "  + 
                                               $"Right now it’s {weather.Weather.FirstOrDefault()?.LongDescription ?? "No current weather data"} " +
                                               $"and the temperature feels like {formattedTemperature}°C degrees, so {outfitSuggestion}{sunglasses}";
            }
            else
            {
                recommendation.ReturnMessage = "We're unable to provide a recommendation due to incomplete weather data.";
            }

            return recommendation;
        }

        private string GetOutfitSuggestion(double tempCelsius, bool isRaining, double windSpeed, Preferences preference)
        {
            //1: FIRST INTERVALL: BELOW -5 CELSIUS 
            string suggestion = "";
            if (tempCelsius <= -5)
            {
                suggestion = "considering that it's freezing outside you should wear your best winter jacket!";
            }
            //2. SECOND INTERVALL: BETWEEN -5 AND +5 CELSIUS
            else if (tempCelsius <= 5)
            {
                suggestion = "considering that it's cold outside you'll need a warm jacket today.";
                if (isRaining) { suggestion += " Since it's raining, we recommend that the jacket is waterproof."; }
                if (windSpeed >= 5) { suggestion += " Since it's windy, we recommend that the jacket is windproof."; }
            }
            //3. THIRD INTERVALL:BETWEEN +5 AND +15 CELSIUS
            else if (tempCelsius <= 15)
            {
                suggestion = "you should wear a jacket today.";
                if (isRaining) { suggestion += " Since it's raining, we recommend that the jacket is waterproof."; }
                if (windSpeed >= 5) { suggestion += " Since it's windy, we recommend that the jacket is windproof."; }
            }
            //4. FOURTH INTERVALL: BETWEEN +15 AND 20 CELSIUS
            else if (tempCelsius <= 20)
            {
                suggestion = preference.Thermal_preference == "Usually Freezing"
                             ? "it’s quite warm outside but we recommend you bring a jacket since you’re usually freezing."
                             : "it's quite warm outside but consider bringing something long-sleeve just in case.";
            }
            //5. FIFTH INTERVALL: OVER 20 CELSIUS
            else
            {
                suggestion = "it’s warm outside so there’s no need for a jacket right now!";
                if (isRaining) { suggestion += " However, since it's raining you should consider bringing an umbrella :)"; }
            }

            return suggestion;
        }

        //METHOD TO ADD A COMMENT ABOUT THE ACTIVITY
        public string ActivityComment(int id, WeatherService ws)
        {
            var latestPreference = _context.Preferences
                                   .Where(p => p.User_Id == id)
                                   .OrderByDescending(p => p.Preference_Id)
                                   .FirstOrDefault();


            string comment = "";


            if (latestPreference.Activity != null)
            {
                if (latestPreference.Activity == "City Stroll")
                { comment = $"Bring a camera so you can take some nice photos during your city stroll"; }

                if (latestPreference.Activity == "Outdoor Activity")
                { comment = $"Enjoy your outdoor activity!"; } 

                if (latestPreference.Activity == "Outdoor Sport")
                { comment = $"Don’t forget to bring a snack and your waterbottle to stay hydrated! ;) "; }
            }

            return comment;
        }

    }
}



