using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Grupp23_CV.Models;
using Microsoft.AspNetCore.Authorization;

namespace Grupp23_CV.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        public AccountController(UserManager<User> userMngr,
        SignInManager<User> signInMngr)
        {
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
                User anvandare = new User
                {
                    UserName = registerViewModel.AnvandarNamn
                };

                var result = await userManager.CreateAsync(anvandare, registerViewModel.Losenord);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(anvandare, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
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

    }
}
