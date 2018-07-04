using StructureMap;
using System.IO;
using SFA.DAS.Forecasting.Application.Infrastructure.Registries;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Domain.Balance;
using Microsoft.Azure.WebJobs;
using System.Collections.Generic;
using SFA.DAS.Forecasting.Domain.Balance.Services;

namespace SFA.DAS.Forecasting.Functions.Framework.Infrastructure
{
    public class ContainerBootstrapper : Registry
    {
        private static readonly object LockObject = new object();
        private static IContainer _container;
        public static IContainer Bootstrap(TraceWriter writer, ExecutionContext executionContext)
        {
            lock (LockObject)
            {
                return _container ?? (_container = new Container(c =>
                {
                    c.AddRegistry<ConfigurationRegistry>();
                    c.AddRegistry<DefaultRegistry>();
                    c.AddRegistry<MediatrRegistry>();
                    c.AddRegistry<DocumentRegistry>();
                    var binPath = Path.Combine(executionContext.FunctionAppDirectory, "bin");
                    writer.Verbose($"Root function bin: {binPath}");
                    var binFolders = new List<string>() { binPath };
                    binFolders.AddRange(Directory.GetDirectories(binPath, "*.", SearchOption.AllDirectories));
                    c.Scan(assScanner =>
                    {
                        //assScanner.LookForRegistries();   
                        assScanner.AssemblyContainingType<CurrentBalance>();
                        assScanner.AssemblyContainingType<IAccountBalanceService>();
                        assScanner.TheCallingAssembly();
                        foreach (var folder in binFolders)
                        {
                            assScanner.AssembliesFromPath(folder, a => a.GetName().Name.StartsWith("SFA.DAS.Forecasting"));
                        }
                        assScanner.RegisterConcreteTypesAgainstTheFirstInterface();
                        assScanner.WithDefaultConventions();

                    });
                }));
            }
        }
    }
}
