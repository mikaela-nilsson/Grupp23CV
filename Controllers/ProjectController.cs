using Grupp23_CV.Database;
using Grupp23_CV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Grupp23_CV.Controllers

{
    public class ProjectController : Controller
    {

        private readonly ApplicationUserDbContext _context;

        // Context skickas in som parameter när controllern skapas, representerar en instans av klassenApplicationUserDdbContext
        // som interagera med databasen, t.ex. för att hämta, skapa, uppdatera eller ta bort data.

        public ProjectController(ApplicationUserDbContext context)
        {
            _context = context;
        }

        //Metod som returnerar ett resultat/vy och en lista av typen project deklareras som lagrar projekten som ska visas i vyn
        //e

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


                // Hämtar alla projekt som är kopplade till den inloggade användaren från databasen.
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

            //Skickar projektlistan till den tillhörande vyn, där den visas för användaren.
            return View(projectLista);
        }



        //När denna metod anropas, returneras den vy där man skapar ett projekt
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //När ett projekt skapas måste vi först lägga till projektet i Projects-tabellen och
        //sedan skapa en post i UserProject för att koppla det till den inloggade användaren.
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

            return View(userProject.Project); // Returnera projektet som ska uppdateras
        }


        [HttpPost]
        public IActionResult Update(Project project)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                // Kontrollera om projektet tillhör användaren
                var userProject = _context.Userprojects
                    .Include(up => up.Project)
                    .FirstOrDefault(up => up.UserId == userId && up.ProjectId == project.ProjectId);

                if (userProject != null)
                {
                    userProject.Project.Title = project.Title;
                    userProject.Project.Description = project.Description;
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }

                return NotFound("Projektet hittades inte eller tillhör inte dig.");
            }

            return View(project);
        }

     

        public IActionResult AllProjects()
        {
            // Hämta alla projekt
            var allProjects = _context.Projects
                .Include(p => p.UserProjects) // Ladda användarrelationer
                .ThenInclude(up => up.User)  // Ladda användarobjekt
                .ToList();

            return View(allProjects); // Skicka alla projekt till vyn
        }



        //[HttpPost]
        //public IActionResult JoinProject(int id, string redirectTo = "AllProjects")
        //{
        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        return Unauthorized("Du måste vara inloggad för att koppla dig till ett projekt.");
        //    }

        //    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        //    // Kontrollera om användaren redan är kopplad till projektet
        //    var existingConnection = _context.Userprojects
        //        .FirstOrDefault(up => up.UserId == userId && up.ProjectId == id);

        //    if (existingConnection == null)
        //    {
        //        var userProject = new UserProject
        //        {
        //            UserId = userId,
        //            ProjectId = id
        //        };
        //        _context.Userprojects.Add(userProject);
        //        _context.SaveChanges();

        //        TempData["JoinMessage"] = "Nu har du kopplat projektet till din profil!";
        //        TempData["JoinedProjectId"] = id; // Lägg till projekt-ID
        //    }
        //    else
        //    {
        //        TempData["JoinMessage"] = "Du är redan kopplad till detta projekt.";
        //        TempData["JoinedProjectId"] = id; // Lägg till projekt-ID
        //    }

        //    return RedirectToAction("Index", "Home");
        //}

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
