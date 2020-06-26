using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SharesBrokerAPI.Domain.ExternalAPIs
{
    public interface IRestShareAPIInvoker
    {
        Task<JObject> GetJsonResponceAsync(string companySymbol);
    }
}
