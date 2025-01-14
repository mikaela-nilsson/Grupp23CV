using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Grupp23_CV.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Grupp23_CV.Database;

namespace Grupp23_CV.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private readonly ApplicationUserDbContext _context;
        public AccountController(ApplicationUserDbContext context, UserManager<User> userMngr,
        SignInManager<User> signInMngr)
        {
            _context = context;
            this.userManager = userMngr;
            this.signInManager = signInMngr;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                // Skapa användare och inkludera IsPrivate
                User anvandare = new User
                {
                    UserName = registerViewModel.AnvandarNamn,
                    IsPrivate = registerViewModel.IsPrivate // Lägg till IsPrivate här
                };

                // Skapa användaren
                var result = await userManager.CreateAsync(anvandare, registerViewModel.Losenord);
                if (result.Succeeded)
                {
                    // Logga in användaren automatiskt
                    await signInManager.SignInAsync(anvandare, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Visa eventuella fel
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(registerViewModel);
        }


        [HttpGet]
        public IActionResult LogIn()
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            return View(loginViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                loginViewModel.AnvandarNamn,
                loginViewModel.Losenord,
                isPersistent: loginViewModel.RememberMe,
                lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Fel användarnam/lösenord.");
                }
            }
            return View(loginViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new ChangePasswordViewModel();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);

            var isOldPasswordValid = await userManager.CheckPasswordAsync(user, model.NuvarandeLosenord);
            if (!isOldPasswordValid)
            {
                ModelState.AddModelError(nameof(model.NuvarandeLosenord), "Det nuvarande lösenordet är felaktigt.");
                return View(model);
            }

            var result = await userManager.ChangePasswordAsync(user, model.NuvarandeLosenord, model.NyttLosenord);
            if (result.Succeeded)
            {
                return RedirectToAction("PasswordChanged"); //Skicka användaren till en bekräftelsesida
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult PasswordChanged()
        {
            return View();
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("LogIn", "Account");
            }

            var cv = await _context.CVs.FirstOrDefaultAsync(c => c.UserId == user.Id);

            var model = new EditProfileViewModel
            {
                FullName = cv?.FullName ?? user.UserName, // Använd FullName från CV eller fallback till UserName
                PhoneNumber = cv?.PhoneNumber ?? user.PhoneNumber,
                IsPrivate = user.IsPrivate,
                Adress = cv?.Adress
            };

            return View(model);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("LogIn", "Account");
            }

            var cv = await _context.CVs.FirstOrDefaultAsync(c => c.UserId == user.Id);

            // Uppdatera användarens data
            //Cv.Adress = model.Adress;
            user.IsPrivate = model.IsPrivate;

            // Uppdatera CV-data
            if (cv != null)
            {
                cv.FullName = model.FullName;
                cv.Adress = model.Adress;
                cv.PhoneNumber = model.PhoneNumber;

                _context.CVs.Update(cv);
            }

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

    }
}
