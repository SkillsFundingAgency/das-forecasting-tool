using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Web.Authentication.GovUkSignIn
{
    public class GovUkUser
    {
        [JsonProperty("sub")]
        public string Sub { get; set; }

        [JsonProperty("phone_number_verified")]
        public bool PhoneNumberVerified { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}