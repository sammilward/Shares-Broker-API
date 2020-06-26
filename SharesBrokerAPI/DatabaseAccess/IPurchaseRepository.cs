using SharesBrokerAPI.Contracts.V1.Requests;
using SharesBrokerAPI.Domain.Models;
using System;
using System.Collections.Generic;

namespace SharesBrokerAPI.DatabaseAccess
{
    public interface IPurchaseRepository
    {
        IEnumerable<Purchase> GetAll();
        IEnumerable<Purchase> GetAll(PurchaseFilters purchaseFilters, double rate);
        IEnumerable<Purchase> GetAll(string username);
        IEnumerable<Purchase> GetAll(string username, PurchaseFilters purchaseFilters, double rate);
        Purchase Get(Guid id);
        void Add(Purchase purchase);
    }
}
