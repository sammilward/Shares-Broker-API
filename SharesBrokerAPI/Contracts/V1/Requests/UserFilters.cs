using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharesBrokerAPI.Contracts.V1.Requests
{
    public class UserFilters
    {
        public string Username { get; set; }
        public double? MinWalletValue { get; set; }
        public double? MaxWalletValue { get; set; }
        public string PrefferedCurrency { get; set; }
        public bool? admins { get; set; }
    }
}
