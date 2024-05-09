using System.ComponentModel.DataAnnotations;// Used to access [Key]

namespace WeatherFit.Entities
{
    public class Users
    {
        [Key] public int User_Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        
        //Navigation property for the related preferences
        public virtual ICollection<Preferences> Preferences { get; set; }

    }
    public class CreateUserDto //SIGNUP INPUT
    {
        //Users table
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        //Preference table
        public string Thermal_Preference { get; set; }
        public string Activity { get; set; }
    }

    public class UpdateUserInfoDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

