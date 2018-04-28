using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Data;
using StructureMap;

namespace SFA.DAS.Forecasting.AcceptanceTests.Infrastructure.Registries
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry()
        {
            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                scan.TheCallingAssembly();
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            ForSingletonOf<Config>().Use(new Config());

            For<ForecastingDataContext>()
                .Use<ForecastingDataContext>()
                .Ctor<string>("connectionString")
                .Is(ctx => ctx.GetInstance<Config>().DatabaseConnectionString)
                .ContainerScoped();

            For<IDbConnection>()
                .Use<SqlConnection>()
                .SelectConstructor(() =>
                    new SqlConnection(ConfigurationManager.ConnectionStrings["DatabaseConnectionString"]
                        .ConnectionString))
                .Ctor<string>("connectionString")
                .Is(ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString);
        }
    }
}
