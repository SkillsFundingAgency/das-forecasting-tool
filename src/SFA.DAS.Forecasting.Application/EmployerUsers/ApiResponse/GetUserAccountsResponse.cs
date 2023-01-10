using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Application.EmployerUsers.ApiResponse;

public class GetUserAccountsResponse
{
    [JsonProperty]
    public string EmployerUserId { get; set; }
    [JsonProperty]
    public string FirstName { get; set; }
    [JsonProperty]
    public string LastName { get; set; }
    [JsonProperty]
    public string Email { get; set; }
    [JsonProperty("UserAccounts")]
    public List<EmployerIdentifier> UserAccounts { get; set; }
}