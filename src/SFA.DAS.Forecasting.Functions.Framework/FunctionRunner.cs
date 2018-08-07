using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
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
        public static async Task Run<TFunction>(TraceWriter writer, ExecutionContext executionContext, Func<IContainer, ILog, Task> runAction) where TFunction : IFunction
        {
            ILog logger = null;
            try
            {
                var container = ContainerBootstrapper.Bootstrap(writer, executionContext);
                using (var nestedContainer = container.GetNestedContainer())
                {
                    ConfigureContainer<TFunction>(executionContext, writer, container);
                    logger = container.GetInstance<ILog>();
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

        public static void Run<TFunction>(TraceWriter writer, ExecutionContext executionContext, Action<IContainer, ILog> runAction) where TFunction : IFunction
        {
            ILog logger = null;
            try
            {
                var container = ContainerBootstrapper.Bootstrap(writer, executionContext);
                using (var nestedContainer = container.GetNestedContainer())
                {
                    ConfigureContainer<TFunction>(executionContext, writer, container);
                    logger = container.GetInstance<ILog>();
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

        public static TReturn Run<TFunction, TReturn>(TraceWriter writer, ExecutionContext executionContext, Func<IContainer, ILog, TReturn> runAction) where TFunction : IFunction
        {
            try
            {
                var container = ContainerBootstrapper.Bootstrap(writer, executionContext);
                using (var nestedContainer = container.GetNestedContainer())
                {
                    ConfigureContainer<TFunction>(executionContext, writer, container);
                    return runAction(nestedContainer, container.GetInstance<ILog>());
                }
            }
            catch (Exception ex)
            {
                writer.Error($"Error invoking function: {typeof(TFunction)}.", ex: ex);
                throw;
            }
        }

        public static async Task<TReturn> Run<TFunction, TReturn>(TraceWriter writer, ExecutionContext executionContext, Func<IContainer, ILog, Task<TReturn>> runAction) where TFunction : IFunction
        {
            ILog logger = null;
            try
            {
                var container = ContainerBootstrapper.Bootstrap(writer, executionContext);
                using (var nestedContainer = container.GetNestedContainer())
                {
                    ConfigureContainer<TFunction>(executionContext, writer, container);
                    return await runAction(nestedContainer, container.GetInstance<ILog>());
                }
            }
            catch (Exception ex)
            {
                writer.Error($"Error invoking function: {typeof(TFunction)}.", ex: ex);
                throw;
            }
        }

        private static void ConfigureContainer<TFunction>(ExecutionContext executionContext, TraceWriter writer, IContainer container) where TFunction : IFunction
        {
            container.Configure(c =>
            {
                var client = container.GetInstance<TelemetryClient>();
                var context = client.StartOperation<RequestTelemetry>(typeof(TFunction).Name);
                c.For<IOperationHolder<RequestTelemetry>>().Use(context);
                c.For<ITelemetry>().Use<AppInsightsTelemetry>();
                c.For<TraceWriter>().Use(writer);
                c.For<TraceWriterLogger>().Use<TraceWriterLogger>();
                c.For<ILog>().Use<CompositeLogger>();
            });
        }
    }
}
