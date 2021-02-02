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
            try
            {
                var container = ContainerBootstrapper.Bootstrap(writer, executionContext);
                using (var nestedContainer = CreateNestedContainer<TFunction>(writer, container))
                {
                    using (StartTelemetryScope<TFunction>(nestedContainer))  //TODO: might be ok to wait for container to be disposed. in that case can get rid of this scope
                    {
                        await runAction(nestedContainer, nestedContainer.GetInstance<ILog>());
                    }
                }
            }
            catch (Exception ex)
            {
                writer.Error($"Error invoking function: {typeof(TFunction)}.", ex);
                throw;
            }
        }

        public static void Run<TFunction>(TraceWriter writer, ExecutionContext executionContext, Action<IContainer, ILog> runAction) where TFunction : IFunction
        {
            try
            {
                var container = ContainerBootstrapper.Bootstrap(writer, executionContext);
                using (var nestedContainer = CreateNestedContainer<TFunction>(writer, container))
                {
                    using (StartTelemetryScope<TFunction>(nestedContainer))
                    {
                        runAction(nestedContainer, nestedContainer.GetInstance<ILog>());
                    }
                }
            }
            catch (Exception ex)
            {
                writer.Error($"Error invoking function: {typeof(TFunction)}.", ex);
                throw;
            }
        }

        public static TReturn Run<TFunction, TReturn>(TraceWriter writer, ExecutionContext executionContext, Func<IContainer, ILog, TReturn> runAction) where TFunction : IFunction
        {
            try
            {
                var container = ContainerBootstrapper.Bootstrap(writer, executionContext);
                using (var nestedContainer = CreateNestedContainer<TFunction>(writer, container))
                {
                    using (StartTelemetryScope<TFunction>(nestedContainer))
                    {
                        return runAction(nestedContainer, nestedContainer.GetInstance<ILog>());
                    }
                }
            }
            catch (Exception ex)
            {
                writer.Error($"Error invoking function: {typeof(TFunction)}.", ex);
                throw;
            }
        }

        public static async Task<TReturn> Run<TFunction, TReturn>(TraceWriter writer, ExecutionContext executionContext, Func<IContainer, ILog, Task<TReturn>> runAction) where TFunction : IFunction
        {
            try
            {
                var container = ContainerBootstrapper.Bootstrap(writer, executionContext);
                using (var nestedContainer = CreateNestedContainer<TFunction>(writer, container))
                {
                    using (StartTelemetryScope<TFunction>(nestedContainer))
                    {
                        return await runAction(nestedContainer, nestedContainer.GetInstance<ILog>());
                    }
                }
            }
            catch (Exception ex)
            {
                writer.Error($"Error invoking function: {typeof(TFunction)}.", ex);
                throw;
            }
        }

        private static IOperationHolder<RequestTelemetry> StartTelemetryScope<TFunction>(IContainer container)
        {
            return container.GetInstance<TelemetryClient>()
                .StartOperation<RequestTelemetry>(typeof(TFunction).Name);
        }

        private static IContainer CreateNestedContainer<TFunction>(TraceWriter writer, IContainer parentContainer) where TFunction : IFunction
        {
            var nestedContainer = parentContainer.GetNestedContainer();
            nestedContainer.Configure(c =>
           {
               //TODO: Possibly return  
               //var client = nestedContainer.GetInstance<TelemetryClient>();
               //var context = client.StartOperation<RequestTelemetry>(typeof(TFunction).Name);
               //c.For<IOperationHolder<RequestTelemetry>>().Use(context);
               c.For<ITelemetry>().Use<AppInsightsTelemetry>();
               c.For<TraceWriter>().Use(writer);
               c.For<TraceWriterLogger>().Use<TraceWriterLogger>();
               //c.For<ILog>().Use<CompositeLogger>();  //TODO: Move to logging or default registry               
           });
            return nestedContainer;
        }
    }
}
