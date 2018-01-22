using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Levy.Domain;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    public class TestFunction : IFunction
    {
        [FunctionName("TestFunction")]
        public static async Task Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            try
            {
                FunctionRunner.SetUpConfiguration<IConfig, Config>("SFA.DAS.Forecasting.Levy");
                await FunctionRunner.Run<TestFunction>(log, async container => {
                    var logger = container.GetInstance<ILog>();
                    var config = container.GetInstance<IConfig>();
                    logger.Info("Hej info");
                    logger.Trace("Hej Trace");
                    logger.Warn("Hej Warn");
                    logger.Error(new Exception(), "Hej Error");

                    var service = container.GetInstance<ILevyWorker>();
                    await service.Run();
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