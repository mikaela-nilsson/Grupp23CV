using Grupp23_CV.Database;
using Grupp23_CV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Security.Claims;

namespace Grupp23_CV.Controllers

{
    public class ProjectController : Controller
    {

        private readonly ApplicationUserDbContext _context;

        public ProjectController(ApplicationUserDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            List<Project> projectLista;

            // Kontrollera om användaren är inloggad och hämtar då användarens unika ID från användarens inloggningsinformation.
            // Om det ej finns returneras "Unathorized" respons med felmeddelande
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("Användarens ID hittades inte.");
                }


                // Hämtar alla projekt som är kopplade till den inloggade användaren från databasen
                // Filtrerar poster i Userprojects där UserId matchar det angivna userId.
                // Använder relationen mellan Userprojects och Projects för att hämta Project-objekt baserat på ProjectId.
                var userId = int.Parse(userIdClaim.Value);

                projectLista = _context.Userprojects
                    .Where(up => up.UserId == userId)
                    .Select(up => up.Project)
                    .ToList();
            }
            else
            {
                // Hämta alla projekt som endast visar titel och beskrivning för utloggade användare
                // Om användaren inte är inloggad, visas endast projekt som tillhör offentliga profiler.
                 projectLista = _context.Projects
               .Where(p => p.UserProjects.Any(up => !up.User.IsPrivate))
               .ToList();
            }

            
            return View(projectLista);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //Skapar ett projekt, kopplar det till den inloggade användaren genom att skapa en relation i UserProjects. 
        //vid fel läggs meddelande till ModelStade för att infimrera användaren. 

        [HttpPost]
        public IActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Projects.Add(project);
                    _context.SaveChanges();

                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var userProject = new UserProject
                    {
                        UserId = userId,
                        ProjectId = project.ProjectId
                    };

                    _context.Userprojects.Add(userProject);
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Logga eller hantera felet
                    ModelState.AddModelError("", "Ett fel inträffade när projektet skulle sparas.");
                }
            }

            return View(project); // Returnera vyn om validering misslyckas
        }


        //Hämtar projekt som tillhör inloggad användare, kontrollerar om user är kopplat till projektet genom UserProject.
        //Om ej, felmeddelande visas.Annars laddas projektet, skickas till vyn för att uppdateras
        [HttpGet]
        public IActionResult Update(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Kontrollera om projektet tillhör användaren via UserProject
            var userProject = _context.Userprojects
                .Include(up => up.Project) // Ladda det relaterade projektet
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == id);

            if (userProject == null)
            {
                return NotFound("Projektet hittades inte eller tillhör inte dig.");
            }

            return View(userProject.Project); 
        }


        //Uppdaterar projekt som tillhör inloggad användare, genom ändring av titel,beskrivning.
        //Om projekt ej hitttas/ej tillhör användare, returneras ett fel. Annars sparas ändringarna och man omridigeras till index.
        [HttpPost]
        public IActionResult Update(Project project)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                // Kontrollera om projektet tillhör användaren, uppdaterar titel, beskrivning
                var userProject = _context.Userprojects
                    .Include(up => up.Project)
                    .FirstOrDefault(up => up.UserId == userId && up.ProjectId == project.ProjectId);

                //Om projekt hittas och tillhör användaren
                if (userProject != null)
                {
                    userProject.Project.Title = project.Title;
                    userProject.Project.Description = project.Description;
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
                //Visas en 404-sida med "texten"
                return NotFound("Projektet hittades inte eller tillhör inte dig.");
            }
            
            return View(project);
        }


        //Hämtar lista över alla projekt med information om vilka anvädnare som deltar i varje projekt genom UserProjects.
        //Info om användare kopplad till projekt, skickas därefter till vy för att visas för användaren. 
        public IActionResult AllProjects()
        {
            // Hämta alla projekt
            var allProjects = _context.Projects
                .Include(p => p.UserProjects) 
                .ThenInclude(up => up.User)  
                .ToList();

            return View(allProjects);
        }

        //Låter en inloggad användare gå med i projekt, kontrollerar om de redan är kopplade till projektet.
        //Om ej kopplad, läggs de till  projektet och meddelande sparas i TempData,
        //för att informera om resultatet innan användaren ordigieras till nästa vy.

        [HttpPost]
        public IActionResult JoinProject(int id, string redirectTo = "AllProjects")
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Du måste vara inloggad för att koppla dig till ett projekt.");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Kontrollera om användaren redan är kopplad till projektet
            var existingConnection = _context.Userprojects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == id);

            if (existingConnection != null)
            {
                // Användaren är redan med i projektet
                TempData["JoinMessage"] = "Du är redan kopplad till detta projekt.";
                TempData["JoinedProjectId"] = id;
            }
            else
            {
                // Lägg till användaren till projektet
                var userProject = new UserProject
                {
                    UserId = userId,
                    ProjectId = id
                };
                _context.Userprojects.Add(userProject);
                _context.SaveChanges();

                TempData["JoinMessage"] = "Nu har du kopplat projektet till din profil!";
                TempData["JoinedProjectId"] = id;
            }

            return RedirectToAction(redirectTo);
        }




    }
}
