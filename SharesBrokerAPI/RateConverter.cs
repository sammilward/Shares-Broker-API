using CurrencyConverterService;
using System.Threading.Tasks;

namespace SharesBrokerAPI
{
    public class RateConverter
    {
        private const string DefaultCurrencyOfSharesAndWallet = "USD";

        CurrencyConversionWSClient _currencyClient;

        public RateConverter(CurrencyConversionWSClient currencyClient)
        {
            _currencyClient = currencyClient;
        }

        public async Task<double> GetRateDollarsToCurrency(string currencyTo)
        {
            try
            {
                return await GetRateResponse(DefaultCurrencyOfSharesAndWallet, currencyTo);
            }
            catch 
            {
                return 0;
            }
        }

        public async Task<double> GetRateCurrencyToDollars(string currencyFrom)
        {
            try
            {
                return await GetRateResponse(currencyFrom, DefaultCurrencyOfSharesAndWallet);
            }
            catch
            {
                return 0;
            }
        }

        private async Task<double> GetRateResponse(string currencyFrom, string currencyTo)
        {
            var rateResponse = await _currencyClient.GetConversionRateAsync(currencyFrom, currencyTo);
            var rate = rateResponse.@return;
            return rate;
        }
    }
}
