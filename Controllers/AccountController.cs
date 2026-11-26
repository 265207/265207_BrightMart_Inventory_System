
using BrightMartInventorySystem.Data;
using BrightMartInventorySystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BrightMartInventorySystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<BrightMartInventorySystem.Models.User> _hasher;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
            _hasher = new PasswordHasher<BrightMartInventorySystem.Models.User>();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var employee = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (employee == null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            var result = _hasher.VerifyHashedPassword(
                employee,
                employee.PasswordHash,
                password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.UserId.ToString()),
                new Claim(ClaimTypes.Name, employee.Username),
                new Claim(ClaimTypes.Role, employee.Role)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
