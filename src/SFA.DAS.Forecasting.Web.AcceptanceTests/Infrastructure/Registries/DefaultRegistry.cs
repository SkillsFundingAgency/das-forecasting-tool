using StructureMap;

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
        }
    }
}