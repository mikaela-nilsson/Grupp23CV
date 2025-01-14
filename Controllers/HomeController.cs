using System.Diagnostics;
using System.Security.Claims;
using Grupp23_CV.Database;
using Grupp23_CV.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Grupp23_CV.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationUserDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationUserDbContext context, Microsoft.AspNetCore.Identity.UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            IQueryable<CV> cvQuery = _context.CVs.Include(cv => cv.User);

            // Kontrollera om användaren inte är autentiserad
            if (!User.Identity.IsAuthenticated)
            {
                cvQuery = cvQuery.Where(cv => !cv.User.IsPrivate); //Hämtar endast offentliga CV:n om användaren inte är inloggad
            }

            //Hämta det senaste projektet i fallande ordning
            var latestProject = _context.Projects
                .OrderByDescending(p => p.ProjectId)
                .FirstOrDefault();

            //Hämta alla CV:n i fallande ordning
            var cvs = await cvQuery
                .OrderByDescending(cv => cv.Id)
                .ToListAsync();

            var currentUser = await _userManager.GetUserAsync(User);

            // Sätt den aktuella användarens ID om användaren finns, annars sätt till 0
            int currentUserId = currentUser != null ? currentUser.Id : 0;



            var viewModel = new HomeViewModel
            {
                CVs = cvs,
                LatestProject = latestProject,
                CurrentUserId = currentUserId
            };

            return View(viewModel);
        }
    }
}