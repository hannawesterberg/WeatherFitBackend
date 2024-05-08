using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherFit.Data;
using WeatherFit.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WeatherFit.Controllers
{
    [ApiController]//attribute to decorate controller class, indicates that controller is meant to serve HTTP API responses
    [Route("[controller]")]//attribute specifies routing patern for the actions within the controller
    public class PreferencesController : ControllerBase
    {
        private readonly ApplicationDbContext _context; //_tables is a handle to the database context to perform CRUD operations
                                                        //and interact with your database within your controller

        //constructor
        public PreferencesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/Preferences - Create a GET Method to fetch ALL preferences 
        [HttpGet]
        public ActionResult<Preferences> GetAllPreferences()
        {
            var allPref = _context.Preferences.ToList();
            return Ok(allPref);
        }

        // GET: api/Preferences - Create GET method to fetch all preferences from one user 
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
        //NEW! Below needed to get the latest preference 
        //GET: api/latest-preferences/{userId} - GET method to fetch the latest preference of a user

        [HttpGet("latest-preferences/{userId}")]
        public ActionResult<Preferences> GetLatestUserPreferences(int userId)
        {
            var preference = _context.Preferences
                                     .Where(p => p.User_Id == userId)
                                     .OrderByDescending(p => p.Preference_Id) // Assuming newer preferences have a higher ID
                                     .FirstOrDefault();// This will take the latest preference from the user 
            if (preference == null)
                return NotFound("Preferences not found for the user.");
            return Ok(preference);
        }

        // POST: api/Preferences/{id} - Create POST method to add new preference for a user
        [HttpPost("{id}")]
        public ActionResult<Preferences> PostPreference(int id, CreatePreferenceDto createPreferenceDto)
        {
            // Check if the user exists in the database
            var user = _context.Users.FirstOrDefault(u => u.User_Id == id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            // Create a new preference object linked to the user
            var preference = new Preferences
            {
                User_Id = id, // Ensure the preference is linked to the user
                Activity = createPreferenceDto.Activity,
                Thermal_preference = createPreferenceDto.Thermal_preference
            };

            _context.Preferences.Add(preference);
            _context.SaveChanges();

            return CreatedAtAction("GetPreference", new { id = preference.Preference_Id }, preference);
        } 

        // OLD POST: api/Preferences - Create POST method to add new preference
        /*[HttpPost]
        public ActionResult<Preferences> PostPreference( CreatePreferenceDto createPreferenceDto)
        {
            var preference = new Preferences
            {

                Activity = createPreferenceDto.Activity,
                Thermal_preference = createPreferenceDto.Thermal_preference
            };

            _context.Preferences.Add(preference);
            _context.SaveChanges();

            return CreatedAtAction("GetPreference", new { id = preference.Preference_Id }, preference);
        }
        */
        /*
        [HttpPost]
        [Authorize]  // Ensure this method is protected by JWT authentication
        public ActionResult PostPreference(CreatePreferenceDto preferenceDto)
        {
            // Retrieve user ID from JWT token
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

            var preference = new Preferences
            {
                User_Id = userId,
                Activity = preferenceDto.Activity,
                Thermal_preference = preferenceDto.Thermal_preference
            };

            _context.Preferences.Add(preference);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetPreference), new { id = preference.Preference_Id }, preference);
        }
        */

        // PUT: api/Preferences/5 - Create PUT method to update an existing preference
        [HttpPut("{id}")]
        public ActionResult UpdatePreference(int id, UpdatePreferenceDto update)
        {
            var pref = _context.Preferences.FirstOrDefault(p => p.User_Id == id);
            if (pref == null)
            {
                return NotFound("Cannot find user");
            }
            //update the fields fromupdatepreferenceDto object
            if (!string.IsNullOrWhiteSpace(update.Thermal_preference))
            {
                pref.Thermal_preference = update.Thermal_preference;
            }
            if (!string.IsNullOrWhiteSpace(update.Activity))
            {
                pref.Activity = update.Activity;
            }

            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/Preferences/5 - Create DELETE method to delete preference
        [HttpDelete("{id}")]
        public ActionResult DeletePreferenceByUserId(int id)
        {
            var pref = _context.Preferences.Find(id);
            if (pref == null)
            {
                return NotFound();
            }
            _context.Preferences.Remove(pref);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
