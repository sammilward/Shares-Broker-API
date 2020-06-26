using System.Collections.Generic;
using System.Linq;
using SharesBrokerAPI.Contracts.V1.Requests;
using SharesBrokerAPI.Domain.Models;

namespace SharesBrokerAPI.DatabaseAccess
{
    public class SqlUserRepository : IUserRepository
    {
        private readonly AppDbContext context;

        public SqlUserRepository(AppDbContext appDbContext)
        {
            context = appDbContext;
        }

        public void Add(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
        }

        public User Delete(string username)
        {
            var user = Get(username);
            if (user != null)
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }
            return user;
        }

        public User Get(string username)
        {
            return context.Users.FirstOrDefault(x => x.Username == username);
        }

        public IEnumerable<User> GetAll()
        {
            return context.Users;
        }

        public bool IsValidLogin(string username, string password)
        {
            var user = context.Users.FirstOrDefault(x => x.Username == username);
            if (user == null || user.Password != password || user.Username != username) return false;
            return true;
        }

        public User UpdatePrefferedCurrency(string username, string prefferedCurrency)
        {
            var user = context.Users.FirstOrDefault(x => x.Username == username);
            user.PrefferedCurrency = prefferedCurrency;
            context.SaveChanges();
            return user;
        }

        public User Update(string username, UpdateUserRequest updateUserRequest)
        {
            var user = context.Users.FirstOrDefault(x => x.Username == username);
            user.WalletValue += updateUserRequest.ChangeWalletValueAmount.Value;
            context.SaveChanges();
            return user;
        }
    }
}
