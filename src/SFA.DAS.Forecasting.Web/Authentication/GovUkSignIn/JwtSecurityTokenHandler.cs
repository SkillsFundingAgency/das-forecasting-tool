using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.Forecasting.Application.GovUkSignIn;

namespace SFA.DAS.Forecasting.Web.Authentication.GovUkSignIn
{
    public interface IJwtSecurityTokenService
    {
        string CreateToken(ClaimsIdentity claimsIdentity,
            SigningCredentials signingCredentials);
    }
    public class JwtSecurityTokenService : IJwtSecurityTokenService, ISecurityTokenValidator
    {
        private readonly string _clientId;
        private readonly string _audience;

        public JwtSecurityTokenService(GovUkOidcConfiguration configuration)
        {
            _clientId = configuration.ClientId;
            _audience = $"{configuration.BaseUrl}/token";
        }
        public string CreateToken(ClaimsIdentity claimsIdentity,
            SigningCredentials signingCredentials)
        {
            var handler = new JwtSecurityTokenHandler();
            var value = handler.CreateJwtSecurityToken(_clientId, _audience, claimsIdentity, DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(5), DateTime.UtcNow, signingCredentials);

            return value.RawData;
        }


        public bool CanReadToken(string securityToken)
        {
            var handler = new JwtSecurityTokenHandler();

            return handler.CanReadToken(securityToken);
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters,
            out SecurityToken validatedToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var claimsPrincipal = handler.ValidateToken(securityToken, validationParameters, out validatedToken);

            return new ClaimsPrincipal(claimsPrincipal);
        }

        public bool CanValidateToken { get; }
        public int MaximumTokenSizeInBytes { get; set; }
    }
}