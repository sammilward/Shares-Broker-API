using System;
using System.ComponentModel.DataAnnotations;

namespace SharesBrokerAPI.Domain.Models
{
    public class Share
    {
        [Key]
        public string CompanySymbol { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public int? NumberOfShares { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public double Value { get; set; }
        [Required]
        public DateTime LastUpdated { get; set; }
    }
}
