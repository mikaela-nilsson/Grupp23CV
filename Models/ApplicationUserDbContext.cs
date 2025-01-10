using Grupp23_CV.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Grupp23_CV.Database
//Den här context klassen hjälper oss att kommunicera med databasen
{
    public class ApplicationUserDbContext : IdentityDbContext<User, IdentityRole<int>, int>
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

            // Sammansatt primärnyckel för UserProject
            modelBuilder.Entity<UserProject>()
                .HasKey(up => new { up.UserId, up.ProjectId });

            // Relation mellan User och UserProject
            modelBuilder.Entity<UserProject>()
                .HasOne(up => up.User)
                .WithMany(u => u.User_Projects)
                .HasForeignKey(up => up.UserId);

            // Relation mellan Project och UserProject
            modelBuilder.Entity<UserProject>()
                .HasOne(up => up.Project)
                .WithMany(p => p.User_Projects)
                .HasForeignKey(up => up.ProjectId);
        }


    }
}



