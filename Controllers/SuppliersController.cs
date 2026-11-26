using Microsoft.AspNetCore.Authorization;
using BrightMartInventorySystem.Data;
using BrightMartInventorySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrightMartInventorySystem.Controllers
{
    [Authorize]
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuppliersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var suppliers = _context.Suppliers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                suppliers = suppliers.Where(s =>
                    s.SupplierName.Contains(searchString) ||
                    s.ContactPerson.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            return View(await suppliers
                .OrderBy(s => s.SupplierName)
                .ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(supplier);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(
            int id, Supplier supplier)
        {
            if (id != supplier.SupplierId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(supplier);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier != null)
            {
                bool hasProducts = await _context.Products
                    .AnyAsync(p => p.SupplierId == id);

                if (hasProducts)
                {
                    TempData["Error"] =
                        "This supplier cannot be deleted because products are linked to it.";
                }
                else
                {
                    _context.Suppliers.Remove(supplier);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
