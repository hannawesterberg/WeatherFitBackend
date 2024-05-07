using System.IdentityModel.Tokens.Jwt;
using System.Linq; // Used for data manipulation, LINQ = language integrated query 
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc; //used for ControllerBase, Controller, ActionResult - define behavior of API
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WeatherFit.Data;
using WeatherFit.Entities;
using WeatherFit.Helpers;


namespace WeatherFit.Controllers
{
    [ApiController]//attribute to decorate controller class, indicates that controller is meant to serve HTTP API responses
    [Route("[controller]")]//attribute specifies routing patern for the actions within the controller
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;//_tables is a handle to the database context to perform CRUD operations
        //and interact with your database within your controller


        private readonly ILogger<UsersController> _logger; // new 
        //Use it to log information, warnings errors or other data, 

        

        //constructor
        public UsersController(ApplicationDbContext context, ILogger<UsersController> logger)//new Ilogger in constructor 
        {
            _context = context;
            _logger = logger;//new 

        }

        // GET: api/User - Create GET method to fetch ALL users
        [HttpGet]
        public ActionResult<Users> GetAllUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        // GET: api/User - Create GET method to fetch specific user
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

        //POST: Create new user 
        [HttpPost]
        public ActionResult<Users> PostUser(CreateUserDto createUserDto)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var user = new Users
                    {
                        Name = createUserDto.Name,
                        Email = createUserDto.Email,
                        Password = PasswordHelper.HashPassword(createUserDto.Password) //NEW  Hash the password
                    };

                    _context.Users.Add(user);
                    _context.SaveChanges();

                    var preferences = new Preferences
                    {
                        User_Id = user.User_Id, // Assign the newly created User_Id
                        Activity = createUserDto.Activity,
                        Thermal_preference = createUserDto.Thermal_Preference
                    };

                    _context.Preferences.Add(preferences);
                    _context.SaveChanges();

                    transaction.Commit();

                    return CreatedAtAction(nameof(GetUser), new { id = user.User_Id }, user);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(500, "Internal server error: " + ex.Message);
                }
            }
        }

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
                return StatusCode(500, "Internal server error occurred.");
            }
        }








        /*
         [HttpPost("login")]
        public ActionResult Login(LoginDto loginDto)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Email == loginDto.Email && u.Password == PasswordHelper.HashPassword(loginDto.Password));

            if (user == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            var token = GenerateJwtToken(user.User_Id);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("your_secret_key_here"); // Make sure this key is safe and comes from your configuration
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        
        
        [HttpPost("login")]
        public ActionResult Login(LoginDto loginDto)
        {
            var hashedPassword = PasswordHelper.HashPassword(loginDto.Password);
            var user = _context.Users.FirstOrDefault(u => u.Email == loginDto.Email && u.Password == hashedPassword);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok("Login successful.");
        }*/


        /* OLD POST Create new user 
        [HttpPost]
        public ActionResult<Users> PostUser(CreateUserDto createUserDto)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var user = new Users
                    {
                        Name = createUserDto.Name,
                        Email = createUserDto.Email,
                        Password = createUserDto.Password
                    };

                    _context.Users.Add(user);
                    _context.SaveChanges(); // This call will generate the User_Id for the new user

                    var preferences = new Preferences
                    {
                        User_Id = user.User_Id, // Assign the newly created User_Id
                        Activity = createUserDto.Activity,
                        Thermal_preference = createUserDto.Thermal_Preference
                    };

                    _context.Preferences.Add(preferences);
                    _context.SaveChanges();

                    transaction.Commit(); // Commit the transaction

                    return CreatedAtAction(nameof(GetUser), new { id = user.User_Id }, user);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(500, "Internal server error: " + ex.Message);
                }
            }
        }

        

        [HttpPost("login")]
        public ActionResult Login(LoginDto loginDto)
        {
            var user = _context.Users
                       .FirstOrDefault(u => u.Email == loginDto.Email && u.Password == loginDto.Password);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Ideally, here you would generate a token or some form of session identifier
            // For simplicity, we're just returning "Login successful."
            return Ok("Login successful.");
        }
        */

        /* OLD OLD POST CREATE NEW USER 
        // POST: api/Users - Create POST method to add a new user
        [HttpPost]
        public ActionResult<Users> UPostUser(CreateUserDto createUserDto)
        {
            var u = new Users
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Password = createUserDto.Password
            };

            _context.Users.Add(u);
            _context.SaveChanges();

            var p = new Preferences
            {
                User_Id = u.User_Id,
                Activity = createUserDto.Activity,
                Thermal_preference = createUserDto.Thermal_Preference

            };


            return CreatedAtAction(nameof(GetUser), new { id = u.User_Id }, u);
        }
        */

        // Update userinfo (PUT) only Name Email and Password 
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

        // DELETE: api/User/5 - Create DELETE method to delete user
        [HttpDelete("{User_id}")]
        public ActionResult<Users> DeleteUser(int User_Id)
        {
            var user = _context.Users.Find(User_Id);
            if (user == null)
            {
                return NotFound();
            }
            var pref = _context.Preferences.Where(p => p.User_Id == User_Id);
            _context.Users.Remove(user);
            _context.SaveChanges();
            return NoContent();
        }
    }
}