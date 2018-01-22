using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Forecasting.Functions.Framework.Infrastructure;
using SFA.DAS.Forecasting.Functions.Framework.Logging;
using SFA.DAS.Forecasting.Levy.Domain;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Forecasting.Functions.Framework
{
    public class FunctionRunner
    {
        public static async Task Run<TFunction>(TraceWriter writer, Func<IContainer, Task> runAction) where TFunction : IFunction
        {
            try
            {
                FunctionRunner.SetUpConfiguration<IConfig, Config>(typeof(TFunction).Namespace?.Replace(".Functions",string.Empty));
                var container = ContainerBootstrapper.Bootstrap();
                using (container.GetNestedContainer())
                {
                    container.Configure(c =>c.For<ILog>().Use(x => LoggerSetup.Create(writer, x.ParentType)));
                    await runAction(container);
                }
            }
            catch (Exception ex)
            {
                writer.Error($"Error invoking function: {typeof(TFunction)}. Error: {ex}");
                throw;
            }
        }

        public static async Task<TReturn> Run<TFunction, TReturn>(TraceWriter log, Func<IContainer, Task<TReturn>> runAction) where TFunction : IFunction
        {
            try
            {
                var container = ContainerBootstrapper.Bootstrap();
                using (container.GetNestedContainer())
                {
                    container.Configure(c => c.For<TraceWriter>().Use(log));
                    return await runAction(container);
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error invoking function: {typeof(TFunction)}. Error: {ex}");
                throw;
            }
        }

        public static T2 SetUpConfiguration<T1, T2>(string serviceName)  where T2 : class, T1
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = ConfigurationManager.AppSettings["EnvironmentName"];
            }

            var storageConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString", EnvironmentVariableTarget.Process);
            var configurationRepository = new AzureTableStorageConfigurationRepository(storageConnectionString);
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(serviceName, environment, "1.0"));

            
            var result = configurationService.Get<T2>();

            var container = ContainerBootstrapper.Bootstrap();
            using (container.GetNestedContainer())
            {
                container.Configure(c => c.For<T1>().Use(result));
            }

            return result;
            
        }
    }
}
