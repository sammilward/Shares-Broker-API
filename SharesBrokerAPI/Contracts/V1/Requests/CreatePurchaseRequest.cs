namespace SharesBrokerAPI.Contracts.V1.Requests
{
    public class CreatePurchaseRequest
    {
        public string CompanySymbol { get; set; }
        public int Quantity { get; set; }
    }
}
