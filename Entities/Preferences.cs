using System.ComponentModel.DataAnnotations;//Used to access [Key]: denotes a property as the primary key in a database table
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeatherFit.Entities
{
    public class Preferences
    {
        //Should inherit User_Id from Users 

        [Key] public int Preference_Id { get; set; } //primary key

        [Required] public int User_Id { get; set; } // Foreign key for users
        public string Activity { get; set; }
        public string Thermal_preference { get; set; }

        [ForeignKey("User_Id")]
        public Users Users { get; set; }//Navigation property: a preference is related to one user


    }
    public class CreatePreferenceDto
    {
        public string Activity { get; set; }
        public string Thermal_preference { get; set; }
    }

    public class UpdatePreferenceDto
    {
        public string Activity { get; set; }
        public string Thermal_preference { get; set; }
    }


}
