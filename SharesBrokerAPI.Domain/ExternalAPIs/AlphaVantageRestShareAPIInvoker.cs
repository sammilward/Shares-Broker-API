using Newtonsoft.Json.Linq;
using SharesBrokerAPI.Domain.HTTP;
using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace SharesBrokerAPI.Domain.ExternalAPIs
{
    public class AlphaVantageRestShareAPIInvoker : IRestShareAPIInvoker
    {

        IHTTPClientFactory _hTTPClientFactory;
        private NameValueCollection Query;

        private const string BaseURL = "https://www.alphavantage.co/query";
        private const string APIKEY = "APIKEY";
        private const string GetLatestShareAPIFunction = "TIME_SERIES_INTRADAY";
        private const string GetLatestShareAPITimeInterval = "60min";

        public AlphaVantageRestShareAPIInvoker(IHTTPClientFactory HTTPClientFactory)
        {
            _hTTPClientFactory = HTTPClientFactory;
            Query = HttpUtility.ParseQueryString(string.Empty);
        }

        public async Task<JObject> GetJsonResponceAsync(string companySymbol)
        {
            Query["function"] = GetLatestShareAPIFunction;
            Query["interval"] = GetLatestShareAPITimeInterval;
            Query["apikey"] = APIKEY;
            Query["symbol"] = companySymbol;

            var queryString = Query.ToString();
            try
            {
                var response = await _hTTPClientFactory.CreateClient().GetAsync(BaseURL + "?" + queryString);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jResponse = JObject.Parse(content);
                    return jResponse;
                }
            } catch
            {
                return null;
            }
            return null;
        }
    }
}
