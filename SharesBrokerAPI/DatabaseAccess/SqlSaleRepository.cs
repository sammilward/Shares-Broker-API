using System;
using System.Collections.Generic;
using System.Linq;
using SharesBrokerAPI.Contracts.V1.Requests;
using SharesBrokerAPI.Domain.Models;

namespace SharesBrokerAPI.DatabaseAccess
{
    public class SqlSaleRepository : ISaleRepository
    {
        private readonly AppDbContext context;

        public SqlSaleRepository(AppDbContext appDbContext)
        {
            context = appDbContext;
        }

        public void Add(Sale sale)
        {
            context.Sales.Add(sale);
            var share = context.Shares.FirstOrDefault(x => x.CompanySymbol == sale.CompanySymbol);
            share.NumberOfShares += sale.Quantity;
            context.SaveChanges();
        }

        public Sale Get(Guid id)
        {
            return context.Sales.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Sale> GetAll()
        {
            return context.Sales;
        }

        public IEnumerable<Sale> GetAll(string username)
        {
            return context.Sales.Where(x => x.Username == username);
        }

        public IEnumerable<Sale> GetAll(SalesFilters salesFilters, double rate)
        {
            var sales = GetAll();
            if (!string.IsNullOrWhiteSpace(salesFilters.Username)) sales = sales.Where(x => x.Username.ToLower() == salesFilters.Username.ToLower());
            if (!string.IsNullOrWhiteSpace(salesFilters.CompanySymbol)) sales = sales.Where(x => x.CompanySymbol.ToLower() == salesFilters.CompanySymbol.ToLower());
            if (salesFilters.MaxTotalValue.HasValue) sales = sales.Where(x => (x.TotalValue * rate) <= salesFilters.MaxTotalValue.Value);
            if (salesFilters.MinTotalValue.HasValue) sales = sales.Where(x => (x.TotalValue * rate) >= salesFilters.MinTotalValue.Value);
            return sales;
        }

        public IEnumerable<Sale> GetAll(string username, SalesFilters salesFilters, double rate)
        {
            var sales = GetAll(username);
            if (!string.IsNullOrWhiteSpace(salesFilters.Username)) sales = sales.Where(x => x.Username.ToLower() == salesFilters.Username.ToLower());
            if (!string.IsNullOrWhiteSpace(salesFilters.CompanySymbol)) sales = sales.Where(x => x.CompanySymbol.ToLower() == salesFilters.CompanySymbol.ToLower());
            if (salesFilters.MaxTotalValue.HasValue) sales = sales.Where(x => (x.TotalValue * rate) <= salesFilters.MaxTotalValue.Value);
            if (salesFilters.MinTotalValue.HasValue) sales = sales.Where(x => (x.TotalValue * rate) >= salesFilters.MinTotalValue.Value);
            return sales;
        }
    }
}   
