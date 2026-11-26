
using BrightMartInventorySystem.Data;
using BrightMartInventorySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrightMartInventorySystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _hasher;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
            _hasher = new PasswordHasher<User>();
        }

        // Display all registered users
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Users
                .OrderBy(u => u.Username)
                .ToListAsync();

            return View(employees);
        }

        // Display the user registration form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Register a new user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string username,
            string password,
            string role)
        {
            username = username?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("",
                    "Username and password are required.");
                return View();
            }

            if (password.Length < 8)
            {
                ModelState.AddModelError("",
                    "Password must contain at least 8 characters.");
                return View();
            }

            if (await _context.Users.AnyAsync(
                u => u.Username == username))
            {
                ModelState.AddModelError("",
                    "That username is already registered.");
                return View();
            }

            if (role != "Admin" &&
                role != "Employee" &&
                role != "BusinessOwner")
            {
                ModelState.AddModelError("",
                    "Please select a valid user role.");
                return View();
            }

            var employee = new User
            {
                Username = username,
                Role = role
            };

            employee.PasswordHash = _hasher.HashPassword(
                employee,
                password
            );

            _context.Users.Add(employee);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "User account created successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
