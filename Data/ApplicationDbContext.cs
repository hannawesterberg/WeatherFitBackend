using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WeatherFit;
using WeatherFit.Entities;

namespace WeatherFit.Data
{
    public class ApplicationDbContext : DbContext
    {
        //constructor
        public ApplicationDbContext() { } //empty constructure so it can be initilized with abailable services
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        //create objects for our tables 
        public DbSet<Users> Users { get; set; }
        public DbSet<Preferences> Preferences { get; set; }

        //Establish one-to-many relationship between Users and Preferences
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            //no need to configure the relationships here because its already defined by data annotations in entities 

            modelBuilder.Entity<Users>() //method to construct a model, based on users 
                .HasMany(u => u.Preferences) //one user has many preferences 
                .WithOne(p => p.Users) //one preference only has "one user"
                .HasForeignKey(p => p.User_Id);//User_Id connects users and preferences


        }




    }
}
