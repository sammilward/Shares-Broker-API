using SharesBrokerAPI.DatabaseAccess;
using SharesBrokerAPI.Domain.ExternalAPIs;
using SharesBrokerAPI.Domain.HTTP;
using System;
using System.Threading.Tasks;

namespace SharesBrokerAPI
{ 
    public class SharesUpdateHandler
    {
        private readonly IRestShareAPIInvoker _restShareAPIInvoker;
        private readonly IShareRepository _shareRepository;

        public SharesUpdateHandler(IShareRepository shareRepository, IHTTPClientFactory hTTPClientFactory)
        {
            _restShareAPIInvoker = new AlphaVantageRestShareAPIInvoker(hTTPClientFactory);
            _shareRepository = shareRepository;
        }

        public async Task HandleShareUpdateAsync(string companySymbol)
        {
            var jsonRespose = await _restShareAPIInvoker.GetJsonResponceAsync(companySymbol);
            if (jsonRespose != null)
            {
                if (jsonRespose.ContainsKey("Time Series (60min)"))
                {
                    var updatedTime = jsonRespose["Meta Data"]["3. Last Refreshed"];
                    var latestValue = jsonRespose["Time Series (60min)"][updatedTime.ToString()]["4. close"];
                    _shareRepository.UpdateShareValue(companySymbol, double.Parse(latestValue.ToString()), DateTime.Parse(updatedTime.ToString()));
                }
            }
        }
    }
}
