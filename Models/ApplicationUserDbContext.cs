using Grupp23_CV.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Grupp23_CV.Database
//Den här context klassen hjälper oss att kommunicera med databasen
{
    public class ApplicationUserDbContext : DbContext
    {
        public ApplicationUserDbContext(DbContextOptions<ApplicationUserDbContext> options)
            : base(options)
        {

        }

        //Dbset för varje entitetklasser som vi har. Vi skapar alltså samlingar av varje entitet. Entity framework ska skapa flera tabeller utifrån detta.
        public DbSet<CV> CVs { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserProject> Userprojects { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CV>().HasData(
                new CV
                {
                    Id = 1,
                    FullName = "Sara Johansson",
                    Adress = "Tomtagatan",
                    PhoneNumber = "12342",
                    ProfileImagePath = "profile.jpg"
                },
                new CV
                {
                    Id = 2,
                    FullName = "Anna Svensson",
                    Adress = "Blåbärsgatan",
                    PhoneNumber = "47473",
                    ProfileImagePath = "profile.jpg"
                }
                );

                modelBuilder.Entity<CV>().HasData(
                new Education
                {
                    Id= 1,
                    Name= "Systemvetenskapligaprogrammet",
                    Institution= "Örebro Universitet",
                    StartDate= new DateTime (2020,01,01),
                    EndDate= new DateTime (2023,1,1),
                    CvId= 2
                }


                );



        }
    }
}
                
        
