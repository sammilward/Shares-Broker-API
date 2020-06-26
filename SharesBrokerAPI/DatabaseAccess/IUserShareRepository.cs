using SharesBrokerAPI.Contracts.V1.Requests;
using SharesBrokerAPI.Domain.Models;
using System;
using System.Collections.Generic;

namespace SharesBrokerAPI.DatabaseAccess
{
    public interface IUserShareRepository
    {
        UserShare GetWithUsernameAndCompanySymbol(string username, string companySymbol);
        IEnumerable<UserShare> GetAll();
        IEnumerable<UserShare> GetAll(UserShareFilters userShareFilters);
        IEnumerable<UserShare> GetAll(string username);
        IEnumerable<UserShare> GetAll(string username, UserShareFilters userShareFilters);
        UserShare Get(Guid id);
        void Add(UserShare userShare);
        UserShare Delete(Guid id);
        UserShare Update(Guid id, int quatity);
    }
}
