using Microsoft.AspNetCore.Mvc;
using WeatherFit.Entities;
using WeatherFit.Services;
using WeatherFit.Data;

[Route("api/[controller]")]
[ApiController]
public class BusinessLogicController : ControllerBase
{
    private readonly BusinessLogic _businessLogic;
    private readonly WeatherService _weatherService;
    private readonly ApplicationDbContext _context;

    public BusinessLogicController(BusinessLogic businessLogic, WeatherService weatherService, ApplicationDbContext context)
    {
        _businessLogic = businessLogic;
        _weatherService = weatherService;
        _context = context;
    }

    private Users GetUserById(int id)
    { 
        return _context.Users.FirstOrDefault(u => u.User_Id == id);
    }

    private Preferences GetLatestPreference(int id)
    {
         var latestPreference = _context.Preferences
                                   .Where(p => p.User_Id == id)
                                   .OrderByDescending(p => p.Preference_Id)
                                   .FirstOrDefault();
        return latestPreference;

    }
    private Preferences GetUserPreferences(int id)
    {
        return _context.Preferences
            .FirstOrDefault(p => p.User_Id == id);
    }

    //GET: api/BusinessLogic/outfit/{city}/{userId} - Method to get the outfit recommendation 
    [HttpGet("outfit/{city}/{userId:int}")]
    public async Task<IActionResult> GetOutfitRecommendation(string city, int userId)
    {
        if (string.IsNullOrEmpty(city))
        {
            return BadRequest("City is required.");
        }

        try
        {
            var weather = await _weatherService.GetWeatherAsync(city);
            var user = GetUserById(userId);  
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var preferences = GetLatestPreference(user.User_Id);
            if (preferences == null)
            {
                return NotFound("User preferences not found.");
            }

            var recommendation = _businessLogic.GetRecommendation(weather, preferences, user, _weatherService);

            // Fetch and add the activity comment to the recommendation message
            var activityComment = _businessLogic.ActivityComment(userId, _weatherService);
            recommendation.ReturnMessage += " " + activityComment; // Append the activity comment to the existing recommendation message.

            return Ok(recommendation);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error getting recommendation: {ex.Message}");
        }
    }
}