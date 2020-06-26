using System;

namespace SharesBrokerAPI.Domain.Models
{
    public class Sale
    {
        public Guid Id { get; set; }
        public DateTime SaleTime { get; set; }
        public string Username { get; set; }
        public string CompanySymbol { get; set; }
        public int Quantity { get; set; }
        public double TotalValue { get; set; }

        public Sale()
        {
            Id = Guid.NewGuid();
            SaleTime = DateTime.Now;
            TotalValue = 0;
        }
    }
}