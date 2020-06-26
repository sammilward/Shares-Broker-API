using System;
using System.Collections.Generic;
using System.Linq;
using SharesBrokerAPI.Contracts.V1.Requests;
using SharesBrokerAPI.Domain.Models;

namespace SharesBrokerAPI.DatabaseAccess
{
    public class SqlShareRepository : IShareRepository
    {
        private readonly AppDbContext context;

        public SqlShareRepository(AppDbContext appDbContext)
        {
            context = appDbContext;
        }

        public void Add(Share share)
        {
            context.Shares.Add(share);
            context.SaveChanges();
        }

        public IEnumerable<Share> GetAll()
        {
            return context.Shares;    
        }

        public IEnumerable<Share> GetAll(ShareFilters shareFilters, double rate)
        {
            var shares = GetAll();
            if (!string.IsNullOrWhiteSpace(shareFilters.CompanySymbol)) shares = shares.Where(x => x.CompanySymbol.ToLower().Contains(shareFilters.CompanySymbol.ToLower()));
            if (!string.IsNullOrWhiteSpace(shareFilters.CompanyName)) shares = shares.Where(x => x.CompanyName.ToLower().Contains(shareFilters.CompanyName.ToLower()));
            if (shareFilters.MinQuantity.HasValue) shares = shares.Where(x => x.NumberOfShares >= shareFilters.MinQuantity.Value);
            if (shareFilters.MaxQuantity.HasValue) shares = shares.Where(x => x.NumberOfShares <= shareFilters.MaxQuantity.Value);
            if (shareFilters.MinValue.HasValue) shares = shares.Where(x => x.Value * rate >= shareFilters.MinValue.Value);
            if (shareFilters.MaxValue.HasValue) shares = shares.Where(x => x.Value * rate <= shareFilters.MaxValue.Value);
            return shares;
        }

        public Share Get(string companySymbol)
        {
            return context.Shares.FirstOrDefault(x => x.CompanySymbol == companySymbol);
        }

        public void UpdateShareValue(string companySymbol, double value, DateTime updatedTime)
        {
            var Share = Get(companySymbol);
            Share.Value = value;
            Share.LastUpdated = updatedTime;
            context.SaveChanges();
        }
    }
}
