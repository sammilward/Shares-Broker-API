using System.ComponentModel.DataAnnotations;

namespace SharesBrokerAPI.Contracts.V1.Requests
{
    public class CreateUserRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public bool IsAdmin { get; set; }
        public string PreferredCurrency { get; set; }
    }
}
