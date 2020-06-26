namespace SharesBrokerAPI.Contracts.V1.Requests
{
    public class SalesFilters
    {
        public string Username { get; set; }
        public string CompanySymbol { get; set; }
        public double? MinTotalValue { get; set; }
        public double? MaxTotalValue { get; set; }
    }
}
