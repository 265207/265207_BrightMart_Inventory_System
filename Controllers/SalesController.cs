using Microsoft.AspNetCore.Authorization;
using BrightMartInventorySystem.Data;
using BrightMartInventorySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BrightMartInventorySystem.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display all sales
        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales
                .Include(s => s.SaleItems)
                .OrderByDescending(s => s.SaleDateTime)
                .ToListAsync();

            return View(sales);
        }

        // Display the sale form
        public async Task<IActionResult> Create()
        {
            await LoadProducts();
            return View();
        }

        // Record a sale and deduct stock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int productId, int quantitySold)
        {
            if (quantitySold <= 0)
            {
                ModelState.AddModelError("", "Quantity must be greater than zero.");
                await LoadProducts();
                return View();
            }

            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                ModelState.AddModelError("", "Please select a valid product.");
                await LoadProducts();
                return View();
            }

            if (quantitySold > product.QuantityInStock)
            {
                ModelState.AddModelError("",
                    $"Not enough stock. Only {product.QuantityInStock} units are available.");
                await LoadProducts();
                return View();
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                decimal lineTotal = product.UnitPrice * quantitySold;

                var sale = new Sale
                {
                    // Temporary demo operator ID.
                    // User login and authentication will be added separately.
                    UserId = int.Parse(
                        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value
                        ),
                    SaleDateTime = DateTime.Now,
                    TotalAmount = lineTotal,
                    SaleItems = new List<SaleItem>
                    {
                        new SaleItem
                        {
                            ProductId = product.ProductId,
                            QuantitySold = quantitySold,
                            UnitPriceAtSale = product.UnitPrice,
                            LineTotal = lineTotal
                        }
                    }
                };

                product.QuantityInStock -= quantitySold;

                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] =
                    "Sale recorded successfully and stock updated.";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await transaction.RollbackAsync();

                ModelState.AddModelError("",
                    "The sale could not be recorded. Please try again.");

                await LoadProducts();
                return View();
            }
        }

        // View individual sale details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var sale = await _context.Sales
                .Include(s => s.SaleItems)
                .FirstOrDefaultAsync(s => s.SaleId == id);

            if (sale == null)
                return NotFound();

            return View(sale);
        }

        private async Task LoadProducts()
        {
            var products = await _context.Products
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            ViewBag.Products = new SelectList(
                products,
                "ProductId",
                "ProductName"
            );
        }
    }
}
