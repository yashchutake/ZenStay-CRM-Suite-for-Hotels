using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using ZenHotelManagement.WebApp.Models;
using ZenHotelManagement.WebApp.Services.Contracts;

namespace ZenHotelManagement.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ICaptchaService _CaptchaService;


        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ICaptchaService CaptchaService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _CaptchaService = CaptchaService;

        }

        
        [HttpGet]
        public ViewResult Login(string returnUrl = "/")
        {
            ViewBag.CaptchaCode = _CaptchaService.GenerateCaptchaCode();
            return View(new Login { Name = string.Empty, ReturnUrl = returnUrl, Password = string.Empty });
        }


        [HttpPost]
        public async Task<IActionResult> Login(Login model, string captchaInput)
        {

            if (!_CaptchaService.ValidateCaptchaCode(captchaInput))
            {
                ModelState.AddModelError("", "Invalid CAPTCHA.");
                ViewBag.CaptchaCode = _CaptchaService.GenerateCaptchaCode();
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Name);
                if (user != null)
                {
                    await _signInManager.SignOutAsync(); // Signing out any existing user session
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Contains("Administrator"))
                        {
                            return RedirectToAction("Dashboard", "Admin");
                        }
                        else if (roles.Contains("Receptionist"))
                        {
                            return RedirectToAction("Dashboard", "Receptionist");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Access denied.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect credentials.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }
            ViewBag.CaptchaCode = _CaptchaService.GenerateCaptchaCode();
            return View(model);
        }
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            
            // Clear any pending customer creation data
            TempData.Remove("PendingCustomerModel");
            
            // Clear session tracking data for the current session
            var currentSessionId = HttpContext.Session.Id;
            var sessionKeyForData = $"PendingCustomerModel_{currentSessionId}";
            HttpContext.Session.Remove(sessionKeyForData);
            
            return Redirect(returnUrl);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {

                // Check if username already exists
                var existingUserByName = await _userManager.FindByNameAsync(model.Name);
                if (existingUserByName != null)
                {
                    ModelState.AddModelError("Name", "Username already exists.");
                    return View(model);
                }

                // Check if email already exists
                var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(model);
                }

                var user = new IdentityUser { UserName = model.Name, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    //await _signInManager.SignInAsync(user, isPersistent: false);

                    // Always redirect to the admin dashboard
                    return RedirectToAction("Dashboard", "Admin");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }
            return View(model);
        }


    }
}
