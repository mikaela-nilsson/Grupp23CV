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

            // Kontrollera om anv�ndaren inte �r autentiserad
            if (!User.Identity.IsAuthenticated)
            {
                cvQuery = cvQuery.Where(cv => !cv.User.IsPrivate); //H�mtar endast offentliga CV:n om anv�ndaren inte �r inloggad
            }

            //H�mta det senaste projektet i fallande ordning
            var latestProject = _context.Projects
                .OrderByDescending(p => p.ProjectId)
                .FirstOrDefault();

            //H�mta alla CV:n i fallande ordning
            var cvs = await cvQuery
                .OrderByDescending(cv => cv.Id)
                .ToListAsync();

            var currentUser = await _userManager.GetUserAsync(User);

            // S�tt den aktuella anv�ndarens ID om anv�ndaren finns, annars s�tt till 0
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