using System;

namespace SharesBrokerAPI.Contracts.V1.Responces
{
    public class ConvertedResponse
    {
        public Boolean currencyServiceDown { get; set; } = false;
        public string currency { get; set; } = "USD";
        public Object result { get; set; }
    }
}
