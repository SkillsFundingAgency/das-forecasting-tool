using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Azure.Core;
using Azure.Identity;

namespace SFA.DAS.Forecasting.Web.Authentication.GovUkSignIn
{
    public interface IAzureIdentityService
    {
        Task<string> AuthenticationCallback(string authority, string resource, string scope);
    }

    public class AzureIdentityService : IAzureIdentityService
    {
        public async Task<string> AuthenticationCallback(string authority, string resource, string scope)
        {
            var chainedTokenCredential = new ChainedTokenCredential(
                new ManagedIdentityCredential(),
                new AzureCliCredential());

            var token = await chainedTokenCredential.GetTokenAsync(
                new TokenRequestContext(scopes: new[] { "https://vault.azure.net/.default" }));

            return token.Token;
        }
    }
}