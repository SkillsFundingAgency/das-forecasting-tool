using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Application.EmployerUsers.ApiResponse;

public class EmployerIdentifier
{
    [JsonProperty("EncodedAccountId")]
    public string AccountId { get; set; }
    [JsonProperty("DasAccountName")]
    public string EmployerName { get; set; }
    [JsonProperty("Role")]
    public string Role { get; set; }
}