namespace SharesBrokerAPI.Contracts.V1.Requests
{
    public class UpdateUserRequest
    {
        public double? ChangeWalletValueAmount { get; set; }
        public string PrefferedCurrency { get; set; }
    }
}
