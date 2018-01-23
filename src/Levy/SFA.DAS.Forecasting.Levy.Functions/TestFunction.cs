using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Levy.Domain;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    public class TestFunction : IFunction
    {
        [FunctionName("TestFunction")]
        public static async Task Run([TimerTrigger("* * */5 * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return;
            try
            {
                await FunctionRunner.Run<TestFunction>(log, async (container, logger) => {
                    var config = container.GetInstance<IConfig>();
                    logger.Info("Hej info");
                    logger.Trace("Hej Trace");
                    logger.Warn("Hej Warn");
                    logger.Error(new Exception(), "Hej Error");
                });

            }
            catch (Exception ex)
            {
                throw;
            }

            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}