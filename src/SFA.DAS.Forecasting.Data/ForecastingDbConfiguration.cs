using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
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
}