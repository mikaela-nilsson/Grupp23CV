using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Grupp23_CV.Models;
using Grupp23_CV.Database;
using Microsoft.EntityFrameworkCore;

namespace Grupp23_CV.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationUserDbContext _context;

        public UserController(ApplicationUserDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Search()
        {
            return View(new UserSearchViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Search(UserSearchViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isLoggedIn = User.Identity.IsAuthenticated;

            model.Results = await _context.CVs
                    .Include(cv => cv.User)
                    .Where(cv => cv.FullName.Contains(model.SearchQuery))
                    .Where(cv => !cv.User.IsPrivate || isLoggedIn)
                    .ToListAsync();

            model.SearchPerformed = true;

            return View(model); //Skicka alla matchande CV-objekt till view:en
        }


        //Visa en användares profil
        public async Task<IActionResult> Profile(int id)
        {
            var user = await _context.Users
                .Include(u => u.CV)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return View(user); // Skickar användarens data till en vy
        }
    }

}

