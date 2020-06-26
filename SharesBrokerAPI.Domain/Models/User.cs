using System.ComponentModel.DataAnnotations;

namespace SharesBrokerAPI.Domain.Models
{
    public class User
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public string PrefferedCurrency { get; set; }
        public double WalletValue { get; set; } = 0;
        public bool PrefferedCurrencyIsDefault()
        {
            return PrefferedCurrency == "USD";
        }
    }
}
