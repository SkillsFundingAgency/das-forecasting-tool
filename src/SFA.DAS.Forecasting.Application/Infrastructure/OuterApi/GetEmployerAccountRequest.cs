using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi
{
    public class GetEmployerAccountRequest : IGetApiRequest
    {
        private readonly string _userId;
        private readonly string _email;

        public GetEmployerAccountRequest(string userId, string email)
        {
            _userId = userId;
            _email = HttpUtility.UrlEncode(email);
        }

        public string GetUrl => $"accountusers/{_userId}/accounts?email={_email}";
    }

    public class GetUserAccountsResponse
    {
        [JsonProperty("UserAccounts")]
        public List<EmployerIdentifier> UserAccounts { get; set; }
    }

    public class EmployerIdentifier
    {
        [JsonProperty("EncodedAccountId")]
        public string AccountId { get; set; }
        [JsonProperty("DasAccountName")]
        public string EmployerName { get; set; }
        [JsonProperty("Role")]
        public string Role { get; set; }
    }
}
