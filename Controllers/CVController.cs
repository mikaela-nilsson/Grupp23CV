using Grupp23_CV.Database;
using Grupp23_CV.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Grupp23_CV.Controllers
{
    public class CVController : Controller
    {
        private readonly ApplicationUserDbContext _context;

        public CVController(ApplicationUserDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cvLista = _context.CVs.ToList();
            return View(cvLista);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CV model, List<Skill> skills, List<Education> educations, List<Experience> experiences)
        {
            if (ModelState.IsValid)
            {
                _context.CVs.Add(model);
                _context.SaveChanges();

                if (skills != null)
                {
                    foreach (var skill in skills)
                    {
                        skill.CvId = model.Id;
                        _context.Skills.Add(skill);
                    }
                }
                if (educations != null)
                {
                    foreach (var education in educations)
                    {
                        education.CvId = model.Id;
                        _context.Educations.Add(education);
                    }
                }
                if (experiences != null)
                {
                    foreach (var experience in experiences)
                    {
                        experience.CvId = model.Id;
                        _context.Experiences.Add(experience);
                    }
                }
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cv = _context.CVs.Find(id);
            if (cv == null)
            {
                return NotFound();
            }
            return View(cv);
        }
        [HttpPost]
        public IActionResult Edit(CV model)
        {
            if (ModelState.IsValid)
            {
                _context.CVs.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var cv = _context.CVs.Find(id);
            if (cv == null)
            {
                return NotFound();
            }
            return View(cv);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var cv = _context.CVs.Find(id);
            if (cv != null)
            {
                _context.CVs.Remove(cv);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
