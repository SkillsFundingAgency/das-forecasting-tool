using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Functions.Framework.Infrastructure;
using SFA.DAS.Forecasting.Functions.Framework.Logging;
using SFA.DAS.Forecasting.Levy.Application;
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
                var container = ContainerBootstrapper.Bootstrap();
                using (container.GetNestedContainer())
                {
                    container.Configure(c =>c.For<ILog>().Use(x => LoggerSetup.Create(writer, x.ParentType)));
                    
                    // Is there a way to not reference the Domain and Application?
                    container.Configure(c => c.For<ILevyWorker>().Use<LevyWorker>());
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
    }
}
