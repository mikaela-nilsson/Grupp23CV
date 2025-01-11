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

        public ProjectController(ApplicationUserDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Project> projectLista;

            // Kontrollera om användaren är inloggad
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("Användarens ID hittades inte.");
                }

                var userId = int.Parse(userIdClaim.Value);

                // Hämta projekt kopplade till den inloggade användaren
                projectLista = _context.Userprojects
                    .Where(up => up.UserId == userId)
                    .Select(up => up.Project)
                    .ToList();
            }
            else
            {
                // Hämta alla projekt som endast visar titel och beskrivning för utloggade användare
                projectLista = _context.Projects.ToList();
            }

            return View(projectLista);
        }



        //Skapar vyn
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
                .ThenInclude(up => up.User) // Ladda användarobjekt
                .ToList();

            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated; // Kolla om användaren är inloggad
            return View(allProjects); // Skicka alla projekt till vyn
        }
        [HttpPost]
        public IActionResult JoinProject(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Du måste vara inloggad för att kunna gå med i ett projekt.");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Kontrollera om användaren redan är med i projektet
            var existingEntry = _context.Userprojects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == id);

            if (existingEntry == null)
            {
                var userProject = new UserProject
                {
                    UserId = userId,
                    ProjectId = id
                };
                _context.Userprojects.Add(userProject);
                _context.SaveChanges();
            }

            return RedirectToAction("AllProjects");
        }



    }
}

