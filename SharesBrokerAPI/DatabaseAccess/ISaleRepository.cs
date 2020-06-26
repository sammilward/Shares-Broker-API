using SharesBrokerAPI.Contracts.V1.Requests;
using SharesBrokerAPI.Domain.Models;
using System;
using System.Collections.Generic;

namespace SharesBrokerAPI.DatabaseAccess
{
    public interface ISaleRepository
    {
        IEnumerable<Sale> GetAll();
        IEnumerable<Sale> GetAll(SalesFilters salesFilters, double rate);
        IEnumerable<Sale> GetAll(string username);
        IEnumerable<Sale> GetAll(string username, SalesFilters salesFilters, double rate);
        Sale Get(Guid id);
        void Add(Sale sale);
    }
}
