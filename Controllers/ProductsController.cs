using Microsoft.AspNetCore.Authorization;
using BrightMartInventorySystem.Data;
using BrightMartInventorySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BrightMartInventorySystem.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                if (int.TryParse(searchString, out int productId))
                {
                    products = products.Where(p =>
                        p.ProductId == productId ||
                        p.ProductName.Contains(searchString));
                }
                else
                {
                    products = products.Where(p =>
                        p.ProductName.Contains(searchString));
                }
            }

            ViewData["CurrentFilter"] = searchString;

            return View(await products
                .OrderBy(p => p.ProductName)
                .ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["SupplierId"] = new SelectList(
                _context.Suppliers, "SupplierId", "SupplierName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                bool supplierExists = await _context.Suppliers
                    .AnyAsync(s => s.SupplierId == product.SupplierId);

                if (!supplierExists)
                {
                    ModelState.AddModelError(
                        "SupplierId", "Please select a valid supplier.");
                }
                else
                {
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["SupplierId"] = new SelectList(
                _context.Suppliers, "SupplierId",
                "SupplierName", product.SupplierId);

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);

            if (product == null) return NotFound();

            ViewData["SupplierId"] = new SelectList(
                _context.Suppliers, "SupplierId",
                "SupplierName", product.SupplierId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(
            int id, Product product)
        {
            if (id != product.ProductId) return NotFound();

            if (ModelState.IsValid)
            {
                bool supplierExists = await _context.Suppliers
                    .AnyAsync(s => s.SupplierId == product.SupplierId);

                if (!supplierExists)
                {
                    ModelState.AddModelError(
                        "SupplierId", "Please select a valid supplier.");
                }
                else
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["SupplierId"] = new SelectList(
                _context.Suppliers, "SupplierId",
                "SupplierName", product.SupplierId);

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
