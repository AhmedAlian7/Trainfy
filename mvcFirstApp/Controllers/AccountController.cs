using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mvcFirstApp.Models.Entities;
using mvcFirstApp.ViewModels;
using System.Security.Claims;

namespace mvcFirstApp.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public AccountController
            (UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVm, string ReturnUrl = "")
        {

            if (!ModelState.IsValid)
                return View(loginVm);
            var user = await userManager.FindByNameAsync(loginVm.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt, wrong username or password!");
                return View(loginVm);
            }
            var checkPassword = await userManager.CheckPasswordAsync(user, loginVm.Password);   
            if (!checkPassword)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt, wrong username or password!");
                return View(loginVm);
            }

            // Sign in the user
            await signInManager.SignInAsync(user, loginVm.RememberMe);
            if (!string.IsNullOrEmpty(loginVm.ReturnUrl) && Url.IsLocalUrl(loginVm.ReturnUrl))
            {
                return Redirect(loginVm.ReturnUrl);
            }
            return RedirectToAction("Index", "Home");
            
        }

        [HttpGet]
        public IActionResult Register()
        {
            ModelState.Clear();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVm)
        {
            if(!ModelState.IsValid)
                return View(registerVm);

            var existingUserByEmail = await userManager.FindByEmailAsync(registerVm.Email);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError("Email", "An account with this email already exists");
                return View(registerVm);
            }
            var existingUserByUsername = await userManager.FindByNameAsync(registerVm.UserName);
            if (existingUserByUsername != null)
            {
                ModelState.AddModelError("UserName", "This username is already taken");
                return View(registerVm);
            }

            var user = new AppUser
            {
                UserName = registerVm.UserName,
                Email = registerVm.Email,
                FullName = registerVm.FullName 
            };

            var result = await userManager.CreateAsync(user, registerVm.Password);
            if (result.Succeeded)
            {
                // Add claim only after user is created
                await userManager.AddClaimAsync(user, new Claim("FullName", user.FullName));
                // Assign the user to a role if needed

                TempData["SuccessMessage"] = "Registration successful! Please log in with your new account.";
                return RedirectToAction("Login");
            }
            else
            {
                result.Errors.ToList().ForEach(e => ModelState.AddModelError(string.Empty, e.Description));
            }
            return View(registerVm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
