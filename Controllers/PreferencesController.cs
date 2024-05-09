using Microsoft.AspNetCore.Mvc;
using WeatherFit.Data;
using WeatherFit.Entities;


namespace WeatherFit.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PreferencesController : ControllerBase
    {
        private readonly ApplicationDbContext _context; // handle to the database context to perform CRUD operations
                                                        
        public PreferencesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Preferences/{id} - GET method to fetch all preferences from one user 
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Preferences>> GetPreference(int id)
        {

            var pref = _context.Preferences.Where(p => p.Preference_Id == id).ToList();
            if (!pref.Any())
            {
                return NotFound("No preferences found for specific user");
            }
            return Ok(pref);
        }

        //GET: Preferences/latest-preferences/{id} - GET method to fetch the latest preference of a user
        [HttpGet("latest-preferences/{id}")]
        public ActionResult<Preferences> GetLatestUserPreferences(int id)
        {
            var preference = _context.Preferences
                                     .Where(p => p.User_Id == id)
                                     .OrderByDescending(p => p.Preference_Id)
                                     .FirstOrDefault();
            if (preference == null)
                return NotFound("Preferences not found for the user.");
            return Ok(preference);
        }

        // POST: /Preferences/{id} - POST method to add new preference for a user
        [HttpPost("{id}")]
        public ActionResult<Preferences> PostPreference(int id, CreatePreferenceDto createPreferenceDto)
        {
            // Check if the user exists in the database
            var user = _context.Users.FirstOrDefault(u => u.User_Id == id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            var lastPreference = _context.Preferences
                                 .Where(p => p.User_Id == id)
                                 .OrderByDescending(p => p.Preference_Id)
                                 .FirstOrDefault();

            if (lastPreference == null)
            {
                return NotFound("No existing preferences found to use as a basis for thermal preference.");
            }

            // Create a new preference object linked to the user
            var preference = new Preferences
            {
                User_Id = id, // Ensure the preference is linked to the user
                Activity = createPreferenceDto.Activity,
                Thermal_preference = lastPreference.Thermal_preference // Use the last known thermal preference

            };

            _context.Preferences.Add(preference);
            _context.SaveChanges();

            return CreatedAtAction("GetPreference", new { id = preference.Preference_Id }, preference);
        } 

    }
}
