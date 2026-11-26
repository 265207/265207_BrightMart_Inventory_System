
using System;
using System.Collections.Generic;
using System.Net.ServerSentEvents;

namespace BrightMartInventorySystem.Models
{
    public class Sale
    {
        public int SaleId { get; set; }

        public int UserId { get; set; }

        public DateTime SaleDateTime { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public List<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}
