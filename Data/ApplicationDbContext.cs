using Microsoft.EntityFrameworkCore; //Enables migration and simplifying data access 
using WeatherFit.Entities;

namespace WeatherFit.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { } 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Preferences> Preferences { get; set; }


    }
}
