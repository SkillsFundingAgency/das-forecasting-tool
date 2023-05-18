using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;

public class GetAccountBalanceRequest : IGetApiRequest
{
    private readonly string _accountId;

    public GetAccountBalanceRequest(string accountId)
    {
        _accountId = accountId;
    }

    public string GetUrl => $"accounts/{_accountId}/balance";
}

public class GetAccountBalanceResponse
{
    [JsonProperty("balance")]
    public decimal Balance { get; set; }

    [JsonProperty("remainingTransferAllowance")]
    public decimal RemainingTransferAllowance { get; set; }

    [JsonProperty("startingTransferAllowance")]
    public decimal StartingTransferAllowance { get; set; }

    [JsonProperty("transferAllowance")]
    public decimal TransferAllowance { get; set; }

    [JsonProperty("isLevyPayer")]
    public int IsLevyPayer { get; set; }
}