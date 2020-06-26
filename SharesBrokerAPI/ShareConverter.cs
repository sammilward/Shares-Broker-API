using SharesBrokerAPI.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace SharesBrokerAPI
{
    public class ShareConverter
    {
        public IEnumerable<Share> ConvertShares(IEnumerable<Share> shares, string currency, double rate)
        {
            var convertedShares = new List<Share>();

            foreach (var share in shares)
            {
                share.Currency = currency;
                share.Value *= rate;
                convertedShares.Add(share);
            }

            return convertedShares;
        }

        public Share ConvertShares(Share share, string currency, double rate)
        {
            share.Currency = currency;
            share.Value *= rate;
            return share;
        }
    }
}
