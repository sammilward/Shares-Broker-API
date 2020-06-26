using Microsoft.EntityFrameworkCore;
using SharesBrokerAPI.Domain.Models;

namespace SharesBrokerAPI.DatabaseAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Share> Shares { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserShare> UserShares { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Sale> Sales { get; set; }
    }
}
