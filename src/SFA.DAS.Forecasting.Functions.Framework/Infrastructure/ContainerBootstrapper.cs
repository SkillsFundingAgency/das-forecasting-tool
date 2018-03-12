using StructureMap;
using System.IO;
using System.Reflection;
using SFA.DAS.Forecasting.Application.Infrastructure.Registries;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Application.Balance.Services;

namespace SFA.DAS.Forecasting.Functions.Framework.Infrastructure
{
    public class ContainerBootstrapper : Registry
    {
        private static readonly object LockObject = new object();
        private static IContainer _container;
        public static IContainer Bootstrap(TraceWriter writer)
        {
            lock (LockObject)
            {
                return _container ?? (_container = new Container(c =>
                {
                    c.AddRegistry<ConfigurationRegistry>();
                    c.AddRegistry<DefaultRegistry>();
                    c.AddRegistry<MediatrRegistry>();
                    var binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    writer.Verbose($"Scanning {binPath} for registries.");
                    c.Scan(assScanner =>
                    {
                        //assScanner.LookForRegistries();   
                        assScanner.AssemblyContainingType<CurrentBalance>();
                        assScanner.AssemblyContainingType<IAccountBalanceService>();
                        assScanner.TheCallingAssembly();
                        assScanner.AssembliesFromPath(binPath, a => a.GetName().Name.StartsWith("SFA.DAS.Forecasting"));
                        assScanner.RegisterConcreteTypesAgainstTheFirstInterface();
                        assScanner.WithDefaultConventions();
                    });
                }));
            }
        }
    }
}
