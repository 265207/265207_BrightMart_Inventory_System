
using BrightMartInventorySystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BrightMartInventorySystem.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAdminAsync(ApplicationDbContext context)
        {
            if (await context.Users.AnyAsync())
                return;

            var admin = new BrightMartInventorySystem.Models.User
            {
                Username = "admin",
                Role = "Admin"
            };

            var hasher = new PasswordHasher<BrightMartInventorySystem.Models.User>();

            admin.PasswordHash = hasher.HashPassword(
                admin,
                "BrightMart@2026!"
            );

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
