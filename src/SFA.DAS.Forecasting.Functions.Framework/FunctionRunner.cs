using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Functions.Framework.Infrastructure;
using SFA.DAS.Forecasting.Functions.Framework.Logging;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Forecasting.Functions.Framework
{
    public class FunctionRunner
    {
        public static async Task Run<TFunction>(TraceWriter writer, Func<IContainer, ILog, Task> runAction) where TFunction : IFunction
        {
            ILog logger = null;
            try
            {
                SetUpConfiguration<IApplicationConfiguration, ApplicationConfiguration>(typeof(TFunction).Namespace?.Replace(".Functions", string.Empty));
                var container = ContainerBootstrapper.Bootstrap();
                using (container.GetNestedContainer())
                {
                    ConfigureContainer(writer, container);
                    logger = container.GetInstance<ILog>();
                    await runAction(container, logger);
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.Error(ex, $"Error invoking function: {typeof(TFunction)}.");
                else
                    writer.Error($"Error invoking function: {typeof(TFunction)}.", ex: ex);

                throw;
            }
        }

        public static TReturn Run<TFunction, TReturn>(TraceWriter writer, Func<IContainer, ILog, TReturn> runAction) where TFunction : IFunction
        {
            try
            {
                SetUpConfiguration<IApplicationConfiguration, ApplicationConfiguration>(typeof(TFunction).Namespace?.Replace(".Functions", string.Empty));
                var container = ContainerBootstrapper.Bootstrap();
                using (container.GetNestedContainer())
                {
                    container.Configure(c => c.For<ILog>().Use(x => LoggerSetup.Create(writer, x.ParentType)));
                    return runAction(container, container.GetInstance<ILog>());
                }
            }
            catch (Exception ex)
            {
                writer.Error($"Error invoking function: {typeof(TFunction)}.", ex: ex);
                throw;
            }
        }

        public static async Task<TReturn> Run<TFunction, TReturn>(TraceWriter writer, Func<IContainer, ILog, Task<TReturn>> runAction) where TFunction : IFunction
        {
            ILog logger = null;
            try
            {
                SetUpConfiguration<IApplicationConfiguration, ApplicationConfiguration>(typeof(TFunction).Namespace?.Replace(".Functions", string.Empty));
                var container = ContainerBootstrapper.Bootstrap();
                using (container.GetNestedContainer())
                {
                    ConfigureContainer(writer, container);
                    logger = container.GetInstance<ILog>();
                    return await runAction(container, container.GetInstance<ILog>());
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.Error(ex, $"Error invoking function: {typeof(TFunction)}.");
                else
                    writer.Error($"Error invoking function: {typeof(TFunction)}.", ex: ex);
                throw;
            }
        }

        private static void ConfigureContainer(TraceWriter writer, IContainer container)
        {
            var config = container.GetInstance<IApplicationConfiguration>();
            container.Configure(c =>
            {
                c.For<ILog>().Use(x => LoggerSetup.Create(writer, x.ParentType));
                c.ForSingletonOf<IHashingService>()
                    .Use<HashingService.HashingService>()
                    .Ctor<string>("allowedCharacters").Is(config.AllowedHashstringCharacters)
                    .Ctor<string>("hashstring").Is(config.Hashstring);

                //c.For<IHashingService>().Use(new HashingService.HashingService(config.AllowedHashstringCharacters, config.Hashstring));
                c.For<IAccountApiClient>().Use<AccountApiClient>()
                    .Ctor<IAccountApiConfiguration>().Is(config.AccountApi);
            });
        }

        private static T2 SetUpConfiguration<T1, T2>(string serviceName) where T2 : class, T1
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
