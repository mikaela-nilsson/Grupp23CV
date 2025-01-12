using System.Diagnostics;
using Grupp23_CV.Database;
using Grupp23_CV.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Grupp23_CV.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationUserDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationUserDbContext context)
        {
            _logger = logger;
            _context = context; // Tilldela DbContext
        }


        public IActionResult Index()
        {
            // Hämta det senaste upplagda projektet
            var latestProject = _context.Projects
                .OrderByDescending(p => p.ProjectId)
                .FirstOrDefault();

            return View(latestProject); // Skicka projektet till vyn
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
