using System.ComponentModel.DataAnnotations;//Used to access [Key]
using System.ComponentModel.DataAnnotations.Schema; //Used to access [ForeignKey]


namespace WeatherFit.Entities
{
    public class Preferences
    {
        [Key] public int Preference_Id { get; set; } 

        [Required] public int User_Id { get; set; } 
        public string Activity { get; set; }
        public string Thermal_preference { get; set; }

        [ForeignKey("User_Id")]
        public Users Users { get; set; }//Navigation property


    }
    public class CreatePreferenceDto //ADD NEW PREFERENCE TO ACCOUNT 
    {
        public string Activity { get; set; }
    }
}
