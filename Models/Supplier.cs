
using System.ComponentModel.DataAnnotations;

namespace BrightMartInventorySystem.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required]
        public string SupplierName { get; set; } = string.Empty;

        public string ContactPerson { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
