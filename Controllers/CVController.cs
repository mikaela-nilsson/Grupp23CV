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
            var currentUser = await _userManager.GetUserAsync(User);

            // Om användaren är inloggad, lagra deras id
            if (currentUser != null)
            {
                ViewBag.CurrentUserId = currentUser.Id;
            }

            // Hämta CV för användaren med det angivna userId
            var userCV = _context.CVs
                .Include(cv => cv.Educations)
                .Include(cv => cv.Experiences)
                .Include(cv => cv.Skills)
                .FirstOrDefault(cv => cv.UserId == userId);

            // Om CV:t inte finns, omdirigera till skapa-sidan
            if (userCV == null)
            {
                return RedirectToAction("Create");
            }

            return View(userCV);
        }


        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CV model, IFormFile profileImage)
        {
            if (!ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                model.UserId = user.Id;

                if (profileImage != null && profileImage.Length > 0)
                {
                    var fileName = LaddaUppFil(profileImage);

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        model.ProfileImagePath = "/Images/" + fileName;
                    }
                }

                _context.CVs.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("CreateEducation", new { cvId = model.Id });
            }
            return View(model);
        }

        private string LaddaUppFil(IFormFile file)
        {
            string fileName = null;

            if (file != null && file.Length > 0)
            {
                string upload = Path.Combine(_environment.WebRootPath, "Images");

                if (!Directory.Exists(upload))
                {
                    Directory.CreateDirectory(upload);
                }

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
            if (!ModelState.IsValid)
            {
                return View(education);
            }

            if (education.CvId == 0)
            {
                ModelState.AddModelError("CVId", "CV är inte korrekt associerat.");
                return View(education);
            }
            var cv = await _context.CVs
                    .Include(c => c.Educations)
                    .FirstOrDefaultAsync(c => c.Id == education.CvId);

            if (cv == null)
            {
                return NotFound($"Ett CV med ID {education.CvId} kunde inte hittas.");
            }

            _context.Educations.Add(education);
            await _context.SaveChangesAsync();

            return RedirectToAction("CreateExperience", new { cvId = education.CvId });
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
                .Include(c => c.Experiences)
                .FirstOrDefaultAsync(c => c.Id == experience.CvId);

            if (cv == null)
            {
                return NotFound($"Ett CV med ID {experience.CvId} kunde inte hittas.");
            }

            _context.Experiences.Add(experience);
            await _context.SaveChangesAsync();

            return RedirectToAction("CreateSkill", new { cvId = experience.CvId });
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
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == skill.CvId);

            if (cv == null)
            {
                return NotFound($"Ett CV med ID {skill.CvId} kunde inte hittas.");
            }

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = skill.CvId });
        }

        public IActionResult Details(int id)
        {
            var cv = _context.CVs
                .Include(c => c.Educations)
                .Include(c => c.Skills)
                .Include(c => c.Experiences)
                .FirstOrDefault(c => c.Id == id);
            if (cv == null)
            {
                return NotFound();
            }
            return View(cv);
        }

        [HttpPost]
        public IActionResult Complete(int id)
        {
            var cv = _context.CVs.Include(c => c.Educations).FirstOrDefault(c => c.Id == id);
            if (cv == null)
            {
                return NotFound();
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cv = await _context.CVs
                .Include(c => c.Educations)
                .Include(c => c.Experiences)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == id);

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

            if (ModelState.IsValid)
            {
                var exCv = await _context.CVs
                    .Include(c => c.Educations)
                    .Include(c => c.Experiences)
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == model.Id);

                if (exCv == null)
                {
                    return NotFound();
                }

                exCv.FullName = model.FullName;
                exCv.Adress = model.Adress;
                exCv.PhoneNumber = model.PhoneNumber;

                if (profileImage != null && profileImage.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);
                    var path = Path.Combine("wwwroot/images", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await profileImage.CopyToAsync(stream);
                    }
                    exCv.ProfileImagePath = "/images/" + fileName;
                }

                _context.Educations.RemoveRange(exCv.Educations);
                _context.Experiences.RemoveRange(exCv.Experiences);
                _context.Skills.RemoveRange(exCv.Skills);

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
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var cv = _context.CVs
                .Include(c => c.Educations)
                .Include(c => c.Experiences)
                .Include(c => c.Skills)
                .FirstOrDefault(c => c.Id == id);

            if (cv == null)
            {
                return NotFound();
            }
            return View(cv);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cv = await _context.CVs.FindAsync(id);
            if (cv != null)
            {
                _context.CVs.Remove(cv);

                if (!string.IsNullOrEmpty(cv.ProfileImagePath))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, cv.ProfileImagePath.TrimStart('/'));

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                _context.CVs.Remove(cv);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
