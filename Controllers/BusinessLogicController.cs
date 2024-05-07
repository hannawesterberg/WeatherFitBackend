using Microsoft.AspNetCore.Mvc;
using WeatherFit.Entities;
using WeatherFit.Services;
using WeatherFit.Data; // to get direct access to Database context 
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class BusinessLogicController : ControllerBase
{
    private readonly BusinessLogic _businessLogic;
    private readonly WeatherService _weatherService;
    private readonly ApplicationDbContext _context; // to get direct access to the database 

    public BusinessLogicController(BusinessLogic businessLogic, WeatherService weatherService, ApplicationDbContext context)
    {
        _businessLogic = businessLogic;
        _weatherService = weatherService;
        _context = context;
    }

    // GET api/recommendation/outfit
    // GET api/recommendation/outfit
    [HttpGet("outfit/{city}/{userEmail}")]
    public async Task<IActionResult> GetOutfitRecommendation(string city, string userEmail)
    {
        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(userEmail))
        {
            return BadRequest("City and user email are required.");
        }

        try
        {
            var weather = await _weatherService.GetWeatherAsync(city);
            var user = GetUserByEmail(userEmail);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var preferences = GetUserPreferences(user.User_Id);
            if (preferences == null)
            {
                return NotFound("User preferences not found.");
            }

            var recommendation = _businessLogic.GetRecommendation(weather, preferences, user, _weatherService);

            // Fetch and add the activity comment to the recommendation message
            var activityComment = _businessLogic.ActivityComment(preferences, user, _weatherService);
            recommendation.ReturnMessage += " " + activityComment; // Append the activity comment to the existing recommendation message.

            return Ok(recommendation);

        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error getting recommendation: {ex.Message}");
        }
    }

    /*[HttpGet("outfit")]
    public async Task<IActionResult> GetOutfitRecommendation([FromQuery] string city, [FromQuery] string userEmail)
    {
        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(userEmail))
        {
            return BadRequest("City and user email are required.");
        }

        try
        {
            var weather = await _weatherService.GetWeatherAsync(city);
            var user = GetUserByEmail(userEmail);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var preferences = GetUserPreferences(user.User_Id);
            if (preferences == null)
            {
                return NotFound("User preferences not found.");
            }

            var recommendation = _businessLogic.GetRecommendation(weather, preferences, user, _weatherService);

            // Fetch and add the activity comment to the recommendation message
            var activityComment = _businessLogic.ActivityComment(preferences, user, _weatherService);
            recommendation.ReturnMessage += " " + activityComment; // Append the activity comment to the existing recommendation message.

            return Ok(recommendation);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error getting recommendation: {ex.Message}");
        }
    }
    */
    private Users GetUserByEmail(string email)
    {
        // Fetch user by email
        return _context.Users
            .Include(u => u.Preferences)
            .FirstOrDefault(u => u.Email == email);
    }

    private Preferences GetUserPreferences(int userId)
    {
        // Fetch preferences for a user
        return _context.Preferences
            .FirstOrDefault(p => p.User_Id == userId);
    }
}

/*
public async Task<IActionResult> GetOutfitRecommendation([FromQuery] string city, [FromQuery] string userEmail)
{
    try
    {
        var weather = await _weatherService.GetWeatherAsync(city);
        var user = GetUserByEmail(userEmail); // Implement this method to retrieve user data based on email
        var preferences = GetUserPreferences(user); // Implement this method to get user preferences

        var recommendation = _businessLogic.GetRecommendation(weather, preferences, user, _weatherService);
        return Ok(recommendation);
    }
    catch (Exception ex)
    {
        return BadRequest($"Error getting recommendation: {ex.Message}");
    }
}

private Users GetUserByEmail(string email)
    {
        // Logic to retrieve user data from the database
        return new Users(); // Placeholder
    }

    private Preferences GetUserPreferences(Users user)
    {
        // Logic to retrieve user preferences from the database
        return new Preferences(); // Placeholder
    }
}
*/