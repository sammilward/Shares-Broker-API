using SharesBrokerAPI.Contracts.V1.Requests;
using SharesBrokerAPI.Domain.Models;
using System;
using System.Collections.Generic;

namespace SharesBrokerAPI.DatabaseAccess
{
    public interface IShareRepository
    {
        IEnumerable<Share> GetAll();
        IEnumerable<Share> GetAll(ShareFilters shareFilters, double rate);
        Share Get(string companySymbol);
        void Add(Share share);
        void UpdateShareValue(string companySymbol, double value, DateTime updatedTime);
    }
}
