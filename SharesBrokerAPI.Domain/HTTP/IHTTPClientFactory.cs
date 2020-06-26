using System.Net.Http;

namespace SharesBrokerAPI.Domain.HTTP
{
    public interface IHTTPClientFactory
    {
        HttpClient CreateClient();
    }
}
