using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.Forecasting.Core;

namespace SFA.DAS.Forecasting.Data
{
    public class ForecastingDbConfiguration : DbConfiguration
    {
        public ForecastingDbConfiguration()
        {
            SetProviderServices("System.Data.EntityClient",
                SqlProviderServices.Instance);
           SetDefaultConnectionFactory(new SqlConnectionFactory());
        }
    }

    [DbConfigurationType(typeof(ForecastingDbConfiguration))]
    public partial class ForecastingDataContext
    {
        public ForecastingDataContext(IApplicationConnectionStrings config)
            : base(config.DatabaseConnectionString)
        {
            InitializePartial();
        }

        partial void DisposePartial(bool disposing)
        {
            Database?.Connection?.GetType()?.GetField("StateChange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(Database?.Connection, null);
        }
    }

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
                return new SqlConnection("Data Source=(localdb)\\projectsv13;Initial Catalog=SFA.DAS.Forecasting.Database;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True");
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