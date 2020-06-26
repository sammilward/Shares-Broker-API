using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SharesBrokerAPI.Contracts.V1.Requests;
using SharesBrokerAPI.Domain.Models;

namespace SharesBrokerAPI.DatabaseAccess
{
    public class SqlUserShareRepository : IUserShareRepository
    {
        private readonly AppDbContext context;

        public SqlUserShareRepository(AppDbContext appDbContext)
        {
            context = appDbContext;
        }

        public void Add(UserShare userShare)
        {
            context.UserShares.Add(userShare);
            context.SaveChanges();
        }

        public UserShare Delete(Guid id)
        {
            var userShare = Get(id);
            if (userShare != null)
            {
                context.UserShares.Remove(userShare);
                context.SaveChanges();
            }
            return userShare;
        }

        public UserShare GetWithUsernameAndCompanySymbol(string username, string companySymbol)
        {
            return context.UserShares.Include(x => x.Share).FirstOrDefault(x => x.Username == username && x.Share.CompanySymbol == companySymbol);
        }

        public UserShare Get(Guid id)
        {
            return context.UserShares.Include(x => x.Share).FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<UserShare> GetAll()
        {
            return context.UserShares.Include(x => x.Share);
        }

        public IEnumerable<UserShare> GetAll(string username)   
        {
            return context.UserShares.Include(x => x.Share).Where(x => x.Username == username);
        }

        public IEnumerable<UserShare> GetAll(UserShareFilters userShareFilters)
        {
            var userShares = GetAll();
            if (!string.IsNullOrWhiteSpace(userShareFilters.Username)) userShares = userShares.Where(x => x.Username.ToLower().Contains(userShareFilters.Username.ToLower()));
            if (!string.IsNullOrWhiteSpace(userShareFilters.CompanySymbol)) userShares = userShares.Where(x => x.Share.CompanySymbol.ToLower() == userShareFilters.CompanySymbol.ToLower());
            if (userShareFilters.MaxValue.HasValue) userShares = userShares.Where(x => (x.Share.Value * x.Quantity ) <= userShareFilters.MaxValue.Value);
            if (userShareFilters.MinValue.HasValue) userShares = userShares.Where(x => (x.Share.Value * x.Quantity ) >= userShareFilters.MinValue.Value);
            if (userShareFilters.MaxQuantity.HasValue) userShares = userShares.Where(x => x.Quantity <= userShareFilters.MaxQuantity.Value);
            if (userShareFilters.MinQuantity.HasValue) userShares = userShares.Where(x => x.Quantity >= userShareFilters.MinQuantity.Value);
            return userShares;
        }

        public IEnumerable<UserShare> GetAll(string username, UserShareFilters userShareFilters)
        {
            var userShares = GetAll(username);
            if (!string.IsNullOrWhiteSpace(userShareFilters.CompanySymbol)) userShares = userShares.Where(x => x.Share.CompanySymbol.ToLower() == userShareFilters.CompanySymbol.ToLower());
            if (userShareFilters.MaxQuantity.HasValue) userShares = userShares.Where(x => x.Quantity <= userShareFilters.MaxQuantity.Value);
            if (userShareFilters.MinQuantity.HasValue) userShares = userShares.Where(x => x.Quantity >= userShareFilters.MinQuantity.Value);
            return userShares;
        }

        public UserShare Update(Guid id, int quantity)
        {
            var userShare = context.UserShares.FirstOrDefault(x => x.Id == id);
            if (userShare.Quantity + quantity == 0)
            {
                context.UserShares.Remove(userShare);
            }
            else
            {
                userShare.Quantity += quantity;
            }
            context.SaveChanges();
            return userShare;
        }
    }
}
