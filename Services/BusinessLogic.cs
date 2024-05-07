using System;
using System.Drawing;
using WeatherFit.Entities;

namespace WeatherFit.Services
{

    public class Recommendation //so we can instantiate a recommendation object
    {
        public string? ReturnMessage { get; set; } //ta bort ?
    }
    public class BusinessLogic
    {
        //
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
                bool isRaining = weather.Weather.Any(w => w.ShortDescription.Contains("rain")); //Creating variable for raining 

                //CHECK TEMPRATUURE RANGES AND ADJUST SUGGESTIONS ACCORINFLY 
                outfitSuggestion = GetOutfitSuggestion(tempCelsius, isRaining, weather.Wind.WindSpeed, preference);

                recommendation.ReturnMessage = $"Hi {users.Name}, for your {preference.Activity} in {weather.City}, " + //How is this the city?? lookinto
                                               $"here is our recommendation for what to wear:"  + // fix so that we have a new row in the front end 
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
                suggestion = "Considering that it's freezing outside you should wear your best winter jacket!";
            }
            //2. SECOND INTERVALL: BETWEEN -5 AND +5 CELSIUS
            else if (tempCelsius <= 5)
            {
                suggestion = "Considering that it's cold outside you'll need a warm jacket today.";
                if (isRaining) { suggestion += " Since it's raining, we recommend that the jacket is waterproof."; }
                if (windSpeed >= 5) { suggestion += " Since it's windy, we recommend that the jacket is windproof."; }
            }
            //3. THIRD INTERVALL:BETWEEN +5 AND +15 CELSIUS
            else if (tempCelsius <= 15)
            {
                suggestion = "You should wear a jacket today.";
                if (isRaining) { suggestion += " Since it's raining, we recommend that the jacket is waterproof."; }
                if (windSpeed >= 5) { suggestion += " Since it's windy, we recommend that the jacket is windproof."; }
            }
            //4. FOURTH INTERVALL: BETWEEN +15 AND 20 CELSIUS
            else if (tempCelsius <= 20)
            {
                suggestion = preference.Thermal_preference == "Usually Freezing"
                             ? "It’s quite warm outside but we recommend you bring a jacket since you’re usually freezing."
                             : "It's quite warm outside but consider bringing something long-sleeve just in case.";
            }
            //5. FIFTH INTERVALL: OVER 20 CELSIUS
            else
            {
                suggestion = "It’s warm outside so there’s no need for a jacket right now!";
                if (isRaining) { suggestion += " However, since it's raining you should consider bringing an umbrella :)"; }
            }

            return suggestion;
        }

        //METHOD TO ADD A COMMENT ABOUT THE ACTIVITY
        public string ActivityComment(Preferences preferences, Users users, WeatherService ws)
        {
            string comment = "";

            if (preferences.Activity != null)
            {
                if (preferences.Activity == "City Stroll")
                { comment = $"Bring a camera so you can take some nice photos during your city stroll"; }

                if (preferences.Activity == "Outdoor Activity")
                { comment = $"Enjoy your outdoor activity! If the weather right now wasn’t what you hoped for, maybe you can find something else to do from this website [INSERT LINK]"; } //find link? or skip.

                if (preferences.Activity == "Outdoor Sport")
                { comment = $"Don’t forget to bring a snack and your waterbottle to stay hydrated! ;) "; }
            }



            return comment;
        }

    }
}



