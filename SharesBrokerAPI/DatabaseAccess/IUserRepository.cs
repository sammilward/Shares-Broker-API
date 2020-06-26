using SharesBrokerAPI.Contracts.V1.Requests;
using SharesBrokerAPI.Domain.Models;
using System.Collections.Generic;

namespace SharesBrokerAPI.DatabaseAccess
{
    public interface IUserRepository
    {
        bool IsValidLogin(string username, string password);
        IEnumerable<User> GetAll();
        User Get(string username);
        void Add(User user);
        User Delete(string username);
        User UpdatePrefferedCurrency(string username, string prefferedCurrency);
        User Update(string username, UpdateUserRequest updateUserRequest);
    }
}
