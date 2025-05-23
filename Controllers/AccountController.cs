using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WaveSurferMusicApp.Models;

namespace WaveSurferMusicApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var result = await this.signInManager.PasswordSignInAsync(
                userName: loginVM.Email,
                password: loginVM.Password,
                isPersistent: true,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("Identifiant incorrect", "Identifiant incorrect");

                return View(loginVM);
            }
        }

        public IActionResult Subscription()
        {
            return View(new SubscriptionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscription(SubscriptionViewModel subscriptionVM)
        {
            if (!ModelState.IsValid)
            {
                return View(subscriptionVM);
            }

            var user = new User
            {
                Email = subscriptionVM.Email,
                UserName = subscriptionVM.Email,
                FirstName = subscriptionVM.FirstName,
                LastName = subscriptionVM.LastName
            };

            var result = await this.userManager.CreateAsync(user, subscriptionVM.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(item.Code, item.Description);
                }

                return View(subscriptionVM);
            }
        }
    }
}