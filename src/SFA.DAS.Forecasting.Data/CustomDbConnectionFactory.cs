using Microsoft.Azure.Services.AppAuthentication;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace SFA.DAS.Forecasting.Data
{
    public class CustomDbConnectionFactory : IDbConnectionFactory
    {
        private bool _isDevEnviornment;

        public CustomDbConnectionFactory(bool isDevEnvironment)
        {
            _isDevEnviornment = isDevEnvironment;
        }

        private static string AzureResource = "https://database.windows.net/";

        public DbConnection CreateConnection(string nameOrConnectionString)
        {
            if (_isDevEnviornment)
            {
                return new SqlConnection(nameOrConnectionString);
            }
            else
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var accessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result;
                return new SqlConnection
                {
                    ConnectionString = nameOrConnectionString,
                    AccessToken = accessToken,
                };
            }
        }
    }
}
