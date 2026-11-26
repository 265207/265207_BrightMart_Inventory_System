
using System.ComponentModel.DataAnnotations;

namespace BrightMartInventorySystem.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        public int SupplierId { get; set; }

        [Range(0.01, 999999.99)]
        public decimal UnitPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }

        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }
    }
}
