using System.ComponentModel.DataAnnotations;//Used to access [Key]: denotes a property as the primary key in a database table
using System.ComponentModel.DataAnnotations.Schema;


namespace WeatherFit.Entities
{
    public class Users
    {
        //Preferences inherits User_Id from here 

        [Key] public int User_Id { get; set; }//might need to rethink since autogen and not user input
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        //one-to-many relationship - one user can have multiple preferences
        //navigation property for the related preferences

        public virtual ICollection<Preferences> Preferences { get; set; }

    }
    public class CreateUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        //Preference properties so they can be stored in the same request 
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

