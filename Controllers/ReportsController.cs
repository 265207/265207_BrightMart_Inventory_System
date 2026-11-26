using Microsoft.AspNetCore.Authorization;
using BrightMartInventorySystem.Data;
using BrightMartInventorySystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrightMartInventorySystem.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            // Calculate the beginning of the current week (Monday).
            int daysSinceMonday =
                ((int)today.DayOfWeek + 6) % 7;

            DateTime weekStart = today.AddDays(-daysSinceMonday);

            // Retrieve today's sales.
            var dailySales = await _context.Sales
                .Where(s => s.SaleDateTime >= today &&
                            s.SaleDateTime < tomorrow)
                .OrderByDescending(s => s.SaleDateTime)
                .ToListAsync();

            // Retrieve sales from Monday up to tomorrow.
            var weeklySales = await _context.Sales
                .Where(s => s.SaleDateTime >= weekStart &&
                            s.SaleDateTime < tomorrow)
                .OrderByDescending(s => s.SaleDateTime)
                .ToListAsync();

            // Retrieve all inventory.
            var inventory = await _context.Products
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            // Identify products at or below their reorder level.
            var lowStock = await _context.Products
                .Where(p => p.QuantityInStock <= p.ReorderLevel)
                .OrderBy(p => p.QuantityInStock)
                .ToListAsync();

            var report = new ReportsViewModel
            {
                DailySales = dailySales,
                WeeklySales = weeklySales,
                DailySalesCount = dailySales.Count,
                WeeklySalesCount = weeklySales.Count,
                DailySalesTotal = dailySales.Sum(s => s.TotalAmount),
                WeeklySalesTotal = weeklySales.Sum(s => s.TotalAmount),
                Inventory = inventory,
                LowStockProducts = lowStock
            };

            return View(report);
        }
    }
}
