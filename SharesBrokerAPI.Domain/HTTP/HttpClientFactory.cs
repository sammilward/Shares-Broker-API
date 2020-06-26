using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SharesBrokerAPI.Domain.HTTP
{
    public class HttpClientFactory : IHTTPClientFactory
    {
        public HttpClient CreateClient()
        {
            var client = new HttpClient();
            SetupClientDefaults(client);
            return client;
        }

        private HttpClient SetupClientDefaults(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            if (!httpClient.DefaultRequestHeaders.Accept.Contains(new MediaTypeWithQualityHeaderValue("application/json")))
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            httpClient.Timeout = new TimeSpan(0, 0, 1);
            return httpClient;
        }
    }
}
