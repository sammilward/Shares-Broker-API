using System;
using System.Collections.Generic;
using System.Linq;
using SharesBrokerAPI.Contracts.V1.Requests;
using SharesBrokerAPI.Domain.Models;

namespace SharesBrokerAPI.DatabaseAccess
{
    public class SqlPurchaseRepository : IPurchaseRepository
    {
        private readonly AppDbContext context;

        public SqlPurchaseRepository(AppDbContext appDbContext)
        {
            context = appDbContext;
        }

        public void Add(Purchase purchase)
        {
            context.Purchases.Add(purchase);
            var share = context.Shares.FirstOrDefault(x => x.CompanySymbol == purchase.CompanySymbol);
            share.NumberOfShares -= purchase.Quantity;
            context.SaveChanges();
        }

        public Purchase Get(Guid id)
        {
            return context.Purchases.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Purchase> GetAll()
        {
            return context.Purchases;
        }

        public IEnumerable<Purchase> GetAll(string username)
        {
            return context.Purchases.Where(x => x.Username == username);
        }

        public IEnumerable<Purchase> GetAll(PurchaseFilters purchaseFilters, double rate)
        {
            var purchases = GetAll();
            if (!string.IsNullOrWhiteSpace(purchaseFilters.Username)) purchases = purchases.Where(x => x.Username.ToLower() == purchaseFilters.Username.ToLower());
            if (!string.IsNullOrWhiteSpace(purchaseFilters.CompanySymbol)) purchases = purchases.Where(x => x.CompanySymbol.ToLower() == purchaseFilters.CompanySymbol.ToLower());
            if (purchaseFilters.MaxTotalValue.HasValue) purchases = purchases.Where(x => (x.TotalValue * rate) <= purchaseFilters.MaxTotalValue.Value);
            if (purchaseFilters.MinTotalValue.HasValue) purchases = purchases.Where(x => (x.TotalValue * rate) >= purchaseFilters.MinTotalValue.Value);
            return purchases;
        }

        public IEnumerable<Purchase> GetAll(string username, PurchaseFilters purchaseFilters, double rate)
        {
            var purchases = GetAll(username);
            if (!string.IsNullOrWhiteSpace(purchaseFilters.Username)) purchases = purchases.Where(x => x.Username.ToLower() == purchaseFilters.Username.ToLower());
            if (!string.IsNullOrWhiteSpace(purchaseFilters.CompanySymbol)) purchases = purchases.Where(x => x.CompanySymbol.ToLower() == purchaseFilters.CompanySymbol.ToLower());
            if (purchaseFilters.MaxTotalValue.HasValue) purchases = purchases.Where(x => (x.TotalValue * rate) <= purchaseFilters.MaxTotalValue.Value);
            if (purchaseFilters.MinTotalValue.HasValue) purchases = purchases.Where(x => (x.TotalValue * rate) >= purchaseFilters.MinTotalValue.Value);
            return purchases;
        }
    }
}   
