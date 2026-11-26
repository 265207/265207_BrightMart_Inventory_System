
using BrightMartInventorySystem.Models;

namespace BrightMartInventorySystem.ViewModels
{
    public class ReportsViewModel
    {
        public decimal DailySalesTotal { get; set; }
        public decimal WeeklySalesTotal { get; set; }

        public int DailySalesCount { get; set; }
        public int WeeklySalesCount { get; set; }

        public List<Sale> DailySales { get; set; } = new();
        public List<Sale> WeeklySales { get; set; } = new();

        public List<Product> Inventory { get; set; } = new();
        public List<Product> LowStockProducts { get; set; } = new();
    }
}
