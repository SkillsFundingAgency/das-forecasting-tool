using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Payments.Functions.Infrastructure;
using StructureMap;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class FunctionRunner
    {
        public static async Task Run<TFunction>(TraceWriter log, Func<IContainer, Task> runAction) where TFunction: IFunction
        {
            try
            {
                var container = ContainerBootstrapper.Bootstrap();
                using (container.GetNestedContainer())
                {
                    container.Configure(c => c.For<TraceWriter>().Use(log));
                    await runAction(container);
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error invoking function: {typeof(TFunction)}. Error: {ex}");
                throw;
            }
        }

        public static async Task< TReturn> Run<TFunction,TReturn>(TraceWriter log, Func<IContainer, Task<TReturn>> runAction) where TFunction: IFunction
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