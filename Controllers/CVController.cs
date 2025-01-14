using Grupp23_CV.Database;
using Grupp23_CV.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Headers;
using System.Security.Claims;


namespace Grupp23_CV.Controllers
{
    public class CVController : Controller
    {
        private readonly ApplicationUserDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _environment;

        public CVController(ApplicationUserDbContext context, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<IActionResult> Index(int userId)
        {
            //Hämtar den aktuella användaren som är inloggad
            var currentUser = await _userManager.GetUserAsync(User);

            bool isOwnCV = currentUser != null && currentUser.Id == userId;

            // Hämta CV för användaren med det angivna userId och inkludera relaterade data
            var userCV = _context.CVs
                .Include(cv => cv.Educations)
                .Include(cv => cv.Experiences)
                .Include(cv => cv.Skills)
                .FirstOrDefault(cv => cv.UserId == userId);

            // Om CV:t inte finns, omdirigera till skapa-sidan
            if (userCV == null && isOwnCV)
            {
                return RedirectToAction("Create");
            }

            //Hämta alla projekt kopplade till användaren
            var userProjects = await _context.Userprojects
            .Where(up => up.UserId == userId)
            .Include(up => up.Project)
            .Select(up => up.Project)
            .ToListAsync();

            // Skicka både CV och projekten till vyn
            var viewModel = new CVProjectsViewModel
            {
                CV = userCV,
                Projects = userProjects,
                IsOwnCV = isOwnCV
            };

            return View(viewModel);
        }


        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CV model, IFormFile? profileImage)
        {
            //Tar bort validering för dessa fält 
            ModelState.Remove("ProfileImagePath");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                //Hämtar den inloggade användaren
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                //Sätter användarens ID på CV:t
                model.UserId = user.Id;

                if (profileImage != null && profileImage.Length > 0)
                {
                    var fileName = LaddaUppFil(profileImage);

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        model.ProfileImagePath = "/Images/" + fileName;
                    }
                }

                //Lägger till det nya CV:t i databasen
                _context.CVs.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new {userId = model.UserId});
            }
            return View(model);
        }

        private string LaddaUppFil(IFormFile file)
        {
            string fileName = null;

            if (file != null && file.Length > 0)
            {
                string upload = Path.Combine(_environment.WebRootPath, "Images"); //Hämtar sökvägen till katalogen för uppladdningar

                //Skapa katalogen om den inte finns
                if (!Directory.Exists(upload))
                {
                    Directory.CreateDirectory(upload);
                }

                //Generera ett filnamn
                fileName = Guid.NewGuid().ToString() + "-" + Path.GetFileName(file.FileName);

                string filePath = Path.Combine(upload, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return fileName;
        }

        [HttpGet]
        public IActionResult CreateEducation(int cvId)
        {
            ViewBag.CvId = cvId;
            return View(new Education { CvId = cvId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateEducation(Education education)
        {
            //Kontrollerar om modellen är giltig
            if (!ModelState.IsValid)
            {
                return View(education);
            }

            //Kontrollera att utbildning är associerad med ett CV
            if (education.CvId == 0)
            {
                ModelState.AddModelError("CVId", "CV är inte korrekt associerat.");
                return View(education);
            }

            //Hämtar CV:t som utbildningen ska associera med
            var cv = await _context.CVs
                    .Include(cv => cv.Educations)
                    .FirstOrDefaultAsync(cv => cv.Id == education.CvId); //Matcha med det angivna CvId

            if (cv == null)
            {
                return NotFound($"Ett CV med ID {education.CvId} kunde inte hittas.");
            }

            //Lägger till utbildningen i databasen
            _context.Educations.Add(education);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { userId = cv.UserId });
        }

        [HttpGet]
        public IActionResult CreateExperience(int cvId)
        {
            ViewBag.CvId = cvId;
            return View(new Experience { CvId = cvId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateExperience(Experience experience)
        {
            if (!ModelState.IsValid)
            {
                return View(experience);
            }

            if (experience.CvId == 0)
            {
                ModelState.AddModelError("CvId", "CV är inte korrekt associerat.");
                return View(experience);
            }

            var cv = await _context.CVs
                .Include(cv => cv.Experiences)
                .FirstOrDefaultAsync(cv => cv.Id == experience.CvId);

            if (cv == null)
            {
                return NotFound($"Ett CV med ID {experience.CvId} kunde inte hittas.");
            }

            _context.Experiences.Add(experience);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { userId = cv.UserId });
        }

        [HttpGet]
        public IActionResult CreateSkill(int cvId)
        {
            ViewBag.CvId = cvId;
            return View(new Skill { CvId = cvId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSkill(Skill skill)
        {
            if (!ModelState.IsValid)
            {
                return View(skill);
            }

            if (skill.CvId == 0)
            {
                ModelState.AddModelError("CvId", "CV är inte korrekt associerat.");
                return View(skill);
            }

            var cv = await _context.CVs
                .Include(cv => cv.Skills)
                .FirstOrDefaultAsync(cv => cv.Id == skill.CvId);

            if (cv == null)
            {
                return NotFound($"Ett CV med ID {skill.CvId} kunde inte hittas.");
            }

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { userId = cv.UserId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //Hämtar info från databasen
            var cv = await _context.CVs
                .Include(cv => cv.Educations)
                .Include(cv => cv.Experiences)
                .Include(cv => cv.Skills)
                .FirstOrDefaultAsync(cv => cv.Id == id);

            if (cv == null)
            {
                return NotFound("CV:t hittades inte");
            }
            return View(cv);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(CV model, IFormFile profileImage)
        {
            //tar bort valideringsfel
            ModelState.Remove("User");
            ModelState.Remove("ProfileImage");
            ModelState.Remove("ProfileImagePath");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            //Kontrollera om modellen är giltig innan uppdatering
            if (ModelState.IsValid)
            {
                //Hämtar det existerande CV:T från databasen
                var exCv = await _context.CVs
                    .Include(cv => cv.Educations)
                    .Include(cv => cv.Experiences)
                    .Include(cv => cv.Skills)
                    .FirstOrDefaultAsync(cv => cv.Id == model.Id);

                if (exCv == null)
                {
                    return NotFound();
                }

                //Uppdatera grundläggande fält
                exCv.FullName = model.FullName;
                exCv.Adress = model.Adress;
                exCv.PhoneNumber = model.PhoneNumber;

                if (profileImage != null && profileImage.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);
                    var path = Path.Combine("wwwroot/images", fileName);

                    //Spara profilbilden på servern
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await profileImage.CopyToAsync(stream);
                    }
                    exCv.ProfileImagePath = "/images/" + fileName;
                }

                //Tar bort relaterade data
                _context.Educations.RemoveRange(exCv.Educations);
                _context.Experiences.RemoveRange(exCv.Experiences);
                _context.Skills.RemoveRange(exCv.Skills);

                //Lägger till nya data
                if (model.Educations != null)
                {
                    foreach (var education in model.Educations)
                    {
                        exCv.Educations.Add(education);
                    }
                }

                if (model.Experiences != null)
                {
                    foreach (var experience in model.Experiences)
                    {
                        exCv.Experiences.Add(experience);
                    }
                }

                if (model.Skills != null)
                {
                    foreach (var skill in model.Skills)
                    {
                        exCv.Skills.Add(skill);
                    }
                }

                _context.CVs.Update(exCv);


                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new {userId = exCv.UserId});
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var cv = _context.CVs
                .Include(cv => cv.Educations)
                .Include(cv => cv.Experiences)
                .Include(cv => cv.Skills)
                .FirstOrDefault(cv => cv.Id == id);

            if (cv == null)
            {
                return NotFound();
            }
            return View(cv);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Hämtar CV:t från databasen med FindAsync
            var cv = await _context.CVs.FindAsync(id);
            if (cv != null)
            {
                //Tar bort tillhörande profilbilder 
                if (!string.IsNullOrEmpty(cv.ProfileImagePath))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, cv.ProfileImagePath.TrimStart('/'));

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                //Tar bort CV:t från databasen
                _context.CVs.Remove(cv);

                //Sparar ändringar i databasen asynkront
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Home", new { userId = cv.UserId });
        }
    }
}
