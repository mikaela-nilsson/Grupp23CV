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

        // Konstruktor som injicerar databascontext för att möjliggöra databashantering
        public UserController(ApplicationUserDbContext context)
        {
            _context = context;
        }
        // GET-metod: Visar söksidan med ett tomt sökformulär
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            return View(new UserSearchViewModel());
        }

        // POST-metod: Hanterar sökning efter CV:n baserat på användarens sökfråga
        [HttpPost]
        public async Task<IActionResult> Search(UserSearchViewModel model)
        {
            // Kontrollera om modellen är giltig innan fortsättning
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Kontrollera om användaren är inloggad
            bool isLoggedIn = User.Identity.IsAuthenticated;

            // Hämta CV:n som matchar sökfrågan
            model.Results = await _context.CVs
                    .Include(cv => cv.User)
                    .Where(cv => cv.FullName.Contains(model.SearchQuery))
                    .Where(cv => !cv.User.IsPrivate || isLoggedIn)
                    .ToListAsync();

            // Ange att en sökning har utförts
            model.SearchPerformed = true;

            return View(model); //Skicka alla matchande CV-objekt till view:en
        }


        //Visa en användares profil
        public async Task<IActionResult> Profile(int id)
        {
            // Hämta användaren från databasen inklusive kopplat CV
            var user = await _context.Users
                .Include(u => u.CV)
                .FirstOrDefaultAsync(u => u.Id == id);

            // Returnera en error-sida om användaren inte hittas
            if (user == null) return NotFound();

            return View(user); // Skickar användarens data till en vy
        }
    }

}

