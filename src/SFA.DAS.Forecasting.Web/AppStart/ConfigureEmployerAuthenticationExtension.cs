using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Forecasting.Application.EmployerUsers;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.Forecasting.Web.Filters;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.Forecasting.Web;

public static class ConfigureEmployerAuthenticationExtension
{
    public static void AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<ICustomClaims, EmployerAccountPostAuthenticationClaimsHandler>();
        services.AddTransient<IEmployerAccountAuthorisationHandler, EmployerAccountAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, EmployerAccountAuthorizationHandler>();
        services.AddTransient<IEmployerAccountService, EmployerAccountService>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                PolicyNames.HasEmployerAccount
                , policy =>
                {
                    policy.RequireClaim(EmployerClaims.AccountsClaimsTypeIdentifier);
                    policy.Requirements.Add(new EmployerAccountRequirement());
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new AccountActiveRequirement());
                });
        });
    }
}