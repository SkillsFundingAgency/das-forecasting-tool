using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Levy.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class AllEmployerLevyPreLoadFunction : IFunction
    {
        [FunctionName("AllEmployerLevyPreLoadFunction")]
        public static async Task Run([QueueTrigger(QueueNames.PreLoadAllLevyRequest)]string message,
            [Queue(QueueNames.LevyPreLoadRequest)] ICollector<PreLoadLevyMessage> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {

            await FunctionRunner.Run<AllEmployerLevyPreLoadFunction>(writer, executionContext,
                async (container, logger) =>
                {
                    var levyDataService = container.GetInstance<IEmployerDataService>();

                    var employerIds = await levyDataService.GetAllAccounts();

                    var latestPeriod = await levyDataService.GetLatestLevyPeriod();

                    logger.Info($"Found {employerIds.Count} employer(s) for period; Year: {latestPeriod.PayrollYear} Month: {latestPeriod.PayrollMonth}");

                    foreach (var id in employerIds)
                    {
                        outputQueueMessage.Add(
                            new PreLoadLevyMessage
                            {
                                EmployerAccountId = id,
                                PeriodYear = latestPeriod.PayrollYear,
                                PeriodMonth = latestPeriod.PayrollMonth
                            });
                    }

                    logger.Info($"Created {employerIds.Count} {nameof(PreLoadLevyMessage)} messages");
                });
        }
    }
}