using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Functions.Framework.Infrastructure;
using SFA.DAS.Forecasting.Functions.Framework.Logging;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Forecasting.Functions.Framework
{
    public class FunctionRunner
    {
        public static async Task Run<TFunction>(TraceWriter writer, ExecutionContext executionContext, Func<IContainer, IAppInsightsTelemetry, Task> runAction) where TFunction : IFunction
        {
	        IAppInsightsTelemetry logger = null;
            try
            {
                var container = ContainerBootstrapper.Bootstrap(writer, executionContext);
                using (var nestedContainer = container.GetNestedContainer())
                {
                    ConfigureContainer(executionContext, writer, container);
                    logger = container.GetInstance<IAppInsightsTelemetry>();
                    await runAction(nestedContainer, logger);
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

        public static void Run<TFunction>(TraceWriter writer, ExecutionContext executionContext, Action<IContainer, IAppInsightsTelemetry> runAction) where TFunction : IFunction
        {
	        IAppInsightsTelemetry logger = null;
            try
            {
                var container = ContainerBootstrapper.Bootstrap(writer, executionContext);
                using (var nestedContainer = container.GetNestedContainer())
                {
                    ConfigureContainer(executionContext, writer, container);
                    logger = container.GetInstance<IAppInsightsTelemetry>();
                    runAction(nestedContainer, logger);
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

        public static TReturn Run<TFunction, TReturn>(TraceWriter writer, ExecutionContext executionContext, Func<IContainer, IAppInsightsTelemetry, TReturn> runAction) where TFunction : IFunction
        {
            try
            {
                var container = ContainerBootstrapper.Bootstrap(writer, executionContext);
                using (var nestedContainer = container.GetNestedContainer())
                {
                    ConfigureContainer(executionContext, writer, container);
                    return runAction(nestedContainer, container.GetInstance<IAppInsightsTelemetry>());
                }
            }
            catch (Exception ex)
            {
                writer.Error($"Error invoking function: {typeof(TFunction)}.", ex: ex);
                throw;
            }
        }

        public static async Task<TReturn> Run<TFunction, TReturn>(TraceWriter writer, ExecutionContext executionContext, Func<IContainer, IAppInsightsTelemetry, Task<TReturn>> runAction) where TFunction : IFunction
        {
	        IAppInsightsTelemetry logger = null;
            try
            {
                var container = ContainerBootstrapper.Bootstrap(writer, executionContext);
                ConfigureContainer(executionContext, writer, container);
                logger = container.GetInstance<IAppInsightsTelemetry>();
                using (var nestedContainer = container.GetNestedContainer())
                {
                    return await runAction(nestedContainer, logger);
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

        private static void ConfigureContainer(ExecutionContext executionContext, TraceWriter writer, IContainer container)
        {
            container.Configure(c =>
            {
                c.For<ILog>().Use(x => LoggerSetup.Create(executionContext, writer, x.ParentType));
            });
        }
    }
}
