using System;

namespace SharesBrokerAPI.Domain.Models
{
    public class Purchase
    {
        public Guid Id { get; set; }
        public DateTime PurchaseTime { get; set; }
        public string Username { get; set; }
        public string CompanySymbol { get; set; }
        public int Quantity { get; set; }
        public double TotalValue { get; set; }

        public Purchase()
        {
            Id = Guid.NewGuid();
            PurchaseTime = DateTime.Now;
            TotalValue = 0;
        }
    }
}