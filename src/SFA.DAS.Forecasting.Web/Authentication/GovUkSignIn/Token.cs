using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Web.Authentication.GovUkSignIn
{
    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}