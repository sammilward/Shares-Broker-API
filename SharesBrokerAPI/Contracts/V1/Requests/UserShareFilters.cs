namespace SharesBrokerAPI.Contracts.V1.Requests
{
    public class UserShareFilters
    {
        public string Username { get; set; }
        public string CompanySymbol { get; set; }
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
    }
}
