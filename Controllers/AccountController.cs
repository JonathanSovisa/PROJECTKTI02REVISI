using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SampleSecureWeb.Data;
using SampleSecureWeb.Models;
using SampleSecureWeb.ViewModels;

namespace SampleSecureWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUser _userData;

        public AccountController(IUser user)
        {
            _userData = user;
        }

        // GET: AccountController
        public ActionResult Index()
        {
            return View();
        }

        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public ActionResult Register(RegistrationViewModel registrationViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Hash password before storing
                    var hashedPassword = HashPassword(registrationViewModel.Password);

                    var user = new User
                    {
                        Username = registrationViewModel.Username,
                        Password = hashedPassword,
                        RoleName = "contributor"
                    };
                    _userData.Registration(user);

                    // Store success message in TempData
                    TempData["SuccessMessage"] = "Registrasi akun Anda berhasil.";

                    // Redirect to Home Index page
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", "Terjadi kesalahan saat registrasi: " + ex.Message);
            }
            return View(registrationViewModel);
        }

       // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel loginViewModel)
        {
            try
            {
                loginViewModel.ReturnUrl ??= Url.Content("~/");

                var user = new User
                {
                    Username = loginViewModel.Username,
                    Password = loginViewModel.Password
                };

                var loginUser = _userData.Login(user);
                if (loginUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, loginUser.Password))
                {
                    ModelState.AddModelError("", "Login tidak valid.");
                    return View(loginViewModel);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginUser.Username),
                    new Claim(ClaimTypes.Role, loginUser.RoleName)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, 
                    new AuthenticationProperties { IsPersistent = loginViewModel.RememberLogin });

                return LocalRedirect(loginViewModel.ReturnUrl);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Terjadi kesalahan saat login: " + ex.Message);
            }
            return View(loginViewModel);
        }


        // POST: Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Hash password method
        private string HashPassword(string password)
        {
            // Implement password hashing here (e.g., using BCrypt or SHA256)
            return password; // Placeholder, replace with actual hashed password
        }


      // GET: Change Password
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: Change Password
     [HttpPost]
public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
{
    var username = User.Identity?.Name;
    if (string.IsNullOrEmpty(username))
    {
        return RedirectToAction("Login");
    }

    var currentUser = _userData.GetUserByUsername(username);
    if (currentUser == null)
    {
        ModelState.AddModelError("", "User not found");
        return View(model);
    }

    if (ModelState.IsValid)
    {
        if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, currentUser.Password))
        {
            ModelState.AddModelError(string.Empty, "Current password is incorrect.");
            return View(model);
        }

        if (!IsValidPassword(model.NewPassword))
        {
            ModelState.AddModelError(string.Empty, "New password must be at least 12 characters long, and include uppercase, lowercase, and a number.");
            return View(model);
        }

        // Hash password baru dan update pengguna
        currentUser.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        _userData.UpdateUser(currentUser); // Pastikan ini memanggil UpdateUser dengan benar
        ViewBag.Message = "Password changed successfully.";
    }
    return View(model);
}

        private bool IsValidPassword(string password)
        {
            return password.Length >= 12; // Misalnya, minimal 12 karakter
        }
    }
}
