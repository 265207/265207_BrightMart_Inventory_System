
using System.ComponentModel.DataAnnotations;

namespace BrightMartInventorySystem.Models
{
    public class SaleItem
    {
        public int SaleItemId { get; set; }

        public int SaleId { get; set; }

        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int QuantitySold { get; set; }

        public decimal UnitPriceAtSale { get; set; }

        public decimal LineTotal { get; set; }
    }
}
