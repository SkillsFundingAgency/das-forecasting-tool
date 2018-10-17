using StructureMap;
using System.IO;
using SFA.DAS.Forecasting.Application.Infrastructure.Registries;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Domain.Balance;
using Microsoft.Azure.WebJobs;
using System.Collections.Generic;
using NServiceBus;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Shared;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Functions.Framework.Infrastructure.Registries;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.StructureMap;

namespace SFA.DAS.Forecasting.Functions.Framework.Infrastructure
{
    public class ContainerBootstrapper : Registry
    {
        private static readonly object LockObject = new object();
        private static IContainer _container;
        private static IEndpointInstance _endpoint;
        public static IContainer Bootstrap(TraceWriter writer, ExecutionContext executionContext)
        {
            lock (LockObject)
            {
                return _container ?? (_container = ConfigureContainer(writer,executionContext));
            }
        }

        private static IContainer ConfigureContainer(TraceWriter writer, ExecutionContext executionContext)
        {
            var container = new Container(c =>
            {
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<DefaultRegistry>();
                c.AddRegistry<MediatrRegistry>();
                c.AddRegistry<DocumentRegistry>();
                c.AddRegistry<LoggingRegistry>();
                c.AddRegistry<TelemetryRegistry>();
                var binPath = Path.Combine(executionContext.FunctionAppDirectory, "bin");
                writer.Verbose($"Root function bin: {binPath}");
                var binFolders = new List<string>() {binPath};
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


            });

            var config = container.GetInstance<IApplicationConfiguration>();

            var endpointConfiguration = new EndpointConfiguration(config.Events.NServiceBusEndpointName)
                .UseAzureServiceBusTransport(config.Events.NServiceBusUseDevTransport,
                    () => config.NServiceBusConnectionString, r => { })
                .UseErrorQueue()
                .UseInstallers()
                .UseLicense(config.Events.NServiceBusLicense.HtmlDecode())
                .UseNewtonsoftJsonSerializer()
                .UseStructureMapBuilder(container);

            _endpoint = Endpoint.Start(endpointConfiguration).Result;

            container.Configure(c =>
            {
                c.For<IMessageSession>().Use(_endpoint);
            });

            return container;
        }
    }
    
}
