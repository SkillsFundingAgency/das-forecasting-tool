using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.Forecasting.Application.GovUkSignIn
{
    public class GovUkOidcConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string KeyVaultIdentifier { get; set; }
    }
}