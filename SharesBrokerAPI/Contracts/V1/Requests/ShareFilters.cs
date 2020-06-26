namespace SharesBrokerAPI.Contracts.V1.Requests
{
    public class ShareFilters
    {
        public string CompanySymbol { get; set; }
        public string CompanyName { get; set; }
        public double? MaxValue { get; set; }
        public double? MinValue { get; set; }
        public int? MaxQuantity { get; set; }
        public int? MinQuantity { get; set; }
    }
}