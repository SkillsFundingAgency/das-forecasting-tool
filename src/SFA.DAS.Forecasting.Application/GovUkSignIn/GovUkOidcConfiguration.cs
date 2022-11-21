using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Application.GovUkSignIn
{
    public class ConfigurationGov
    {
        [JsonProperty("GovUkOidcConfiguration")]
        public GovUkOidcConfiguration GovUkOidcConfiguration { get; set; }
    }

    public class GovUkOidcConfiguration
    {
        [JsonProperty("BaseUrl")]
        public string BaseUrl { get; set; }

        [JsonProperty("KeyVaultIdentifier")]
        public string KeyVaultIdentifier { get; set; }

        [JsonProperty("ClientId")]
        public string ClientId { get; set; }
    }
}