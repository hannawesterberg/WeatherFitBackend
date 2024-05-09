using Microsoft.AspNetCore.Mvc; //used for ControllerBase, Controller, ActionResult - define behavior of API
using Microsoft.EntityFrameworkCore;
using WeatherFit.Data;
using WeatherFit.Entities;
using WeatherFit.Helpers;


namespace WeatherFit.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context; // Handling database connection to perform CRUD 
        private readonly ILogger<UsersController> _logger; //Used to log info, track errors or other data especially when logging in 

        public UsersController(ApplicationDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }


        // GET: /Users/{id} - GET method to fetch specific user
        [HttpGet("{id}")]
        public ActionResult<Users> GetUser(int id)
        {
            var user = _context.Users
                .Include(u => u.Preferences)
                .FirstOrDefault(u => u.User_Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        //POST: /Users - POST method to create a new user (SIGN UP)
        [HttpPost]
        public ActionResult<Users> PostUser(CreateUserDto createUserDto)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var user = new Users //Users table
                    {
                        Name = createUserDto.Name,
                        Email = createUserDto.Email,
                        Password = PasswordHelper.HashPassword(createUserDto.Password) //Hash Password 
                    };

                    _context.Users.Add(user);
                    _context.SaveChanges();

                    var preferences = new Preferences //Preference table
                    {
                        User_Id = user.User_Id,
                        Activity = createUserDto.Activity,
                        Thermal_preference = createUserDto.Thermal_Preference
                    };

                    _context.Preferences.Add(preferences);
                    _context.SaveChanges();

                    transaction.Commit();

                    // Log the success with user ID and email
                    _logger.LogInformation("New user created successfully with email {Email} and ID {User_Id}", user.Email, user.User_Id);

                    // Include user ID in the response
                    return CreatedAtAction(nameof(GetUser), new { id = user.User_Id }, new { Message = "User created successfully.", UserId = user.User_Id });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError("Error creating user: {Error}", ex.Message); // Log the specific error
                    return StatusCode(500, "Internal server error: " + ex.Message);
                }
            }
        }

        //POST: Users/login - POST method to login existing user (LOG IN) 
        [HttpPost("login")]
        public ActionResult Login(LoginDto loginDto)
        {
            try
            {
                var hashedPassword = PasswordHelper.HashPassword(loginDto.Password);
                var user = _context.Users.FirstOrDefault(u => u.Email == loginDto.Email && u.Password == hashedPassword);

                if (user == null)
                {
                    _logger.LogWarning("Login attempt failed for user {Email}", loginDto.Email);
                    return Unauthorized("Invalid email or password.");
                }
                _logger.LogInformation("User {Email} logged in successfully", loginDto.Email);
                return Ok(new { Message = "Login successful.", UserId = user.User_Id });
            }
            catch (Exception ex)
            {
                // Log the exception details to help with debugging
                _logger.LogError("Login failed: {0}", ex.Message);
                return StatusCode(500, "Internal server error occurred."); //Generic HTTP status code 
            }
        }

        //PUT: User/{id} - Update user name, email and password (ACCOUNT SETTINGS)
        [HttpPut("{id}")]
        public ActionResult<Users> UpdateUser(int id, UpdateUserInfoDto updateuser)
        {
            try
            {
                var user = _context.Users.Find(id);
                if (user == null)
                {
                    return NotFound();
                }

                //Update fields if the new values are not null or whitespace
                user.Name = !string.IsNullOrWhiteSpace(updateuser.Name) ? updateuser.Name : user.Name;
                user.Email = !string.IsNullOrWhiteSpace(updateuser.Email) ? updateuser.Email : user.Email;
                user.Password = !string.IsNullOrWhiteSpace(updateuser.Password) ? updateuser.Password : user.Password;

                _context.SaveChanges();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occured whule updating the user.");
            }

        }

        // DELETE: Users/
        [HttpDelete("{id}")]
        public ActionResult<Users> DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            var pref = _context.Preferences.Where(p => p.User_Id == id);
            _context.Users.Remove(user);
            _context.SaveChanges();
            return NoContent();
        }
    }
}