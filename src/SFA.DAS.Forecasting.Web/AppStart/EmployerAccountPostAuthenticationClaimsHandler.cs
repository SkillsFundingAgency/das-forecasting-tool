using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.EmployerUsers;
using SFA.DAS.Forecasting.Application.EmployerUsers.ApiResponse;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Web.Filters;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.Forecasting.Web;

public class EmployerAccountPostAuthenticationClaimsHandler : ICustomClaims
{
    private readonly IEmployerAccountService _accountsSvc;
    private readonly ForecastingConfiguration _forecastingConfiguration;

    public EmployerAccountPostAuthenticationClaimsHandler(IEmployerAccountService accountsSvc, IOptions<ForecastingConfiguration> forecastingConfiguration)
    {
        _accountsSvc = accountsSvc;
        _forecastingConfiguration = forecastingConfiguration.Value;
    }
    public async Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
    {
        var claims = new List<Claim>();

        var userId = tokenValidatedContext.Principal.Claims
            .First(c => c.Type.Equals(ClaimTypes.NameIdentifier))
            .Value;
        var email = tokenValidatedContext.Principal.Claims
            .First(c => c.Type.Equals(ClaimTypes.Email))
            .Value;
        claims.Add(new Claim(EmployerClaims.IdamsUserEmailClaimTypeIdentifier, email));
        
        var result = await _accountsSvc.GetUserAccounts(userId, email);

        var accountsAsJson = JsonConvert.SerializeObject(result.EmployerAccounts.ToDictionary(k => k.AccountId));
        var associatedAccountsClaim = new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, accountsAsJson, JsonClaimValueTypes.Json);
        claims.Add(associatedAccountsClaim);
            
        claims.Add(new Claim(EmployerClaims.IdamsUserIdClaimTypeIdentifier, result.EmployerUserId));
        claims.Add(new Claim(EmployerClaims.IdamsUserDisplayNameClaimTypeIdentifier, result.FirstName + " " + result.LastName));
        
        if (result.IsSuspended)
        {
            claims.Add(new Claim(ClaimTypes.AuthorizationDecision, "Suspended"));
        }
        
        return claims;
    }
}