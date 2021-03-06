using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Data;
using StructureMap;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.Infrastructure.Registries
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
            ForSingletonOf<Config>();

            For<IForecastingDataContext>()
                .Use<ForecastingDataContext>()
                .Ctor<IApplicationConnectionStrings>("config")
                .Is(ctx => ctx.GetInstance<Config>())
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