using System;

namespace SharesBrokerAPI.Domain.Models
{
    public class UserShare
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public Share Share { get; set; }
        public string Username { get; set; }

        public UserShare()
        {
            Id = Guid.NewGuid();
        }
    }
}
