using Microsoft.AspNetCore.Mvc;

namespace SharesBrokerAPI.Contracts.V1.Headers
{
    public class AuthHeader
    {
        [FromHeader(Name = "username")]
        public string Username { get; set; }
        [FromHeader(Name = "password")]
        public string Password { get; set; }
    }
}
