using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using SFA.DAS.Forecasting.Application.Levy.Messages.PreLoad;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class LevyPreLoadServiceBusFunction : IFunction
    {
        [FunctionName("LevyPreLoadServiceBusFunction")]
        [return: Queue(QueueNames.LevyPreLoadRequest)]
        public static async Task<PreLoadLevyMessage> Run(
            [ServiceBusTrigger("RefreshPaymentDataCompletedEvent", "Forecasting", AccessRights.Listen)]PreLoadLevyRequestMessage preLoadLevyRequestMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<LevyPreLoadServiceBusFunction, PreLoadLevyMessage>(writer, executionContext,
                async (container, logger) =>
                {
                    if (!preLoadLevyRequestMessage.LevyImported)
                    {
                        logger.Info($"No new messages of type {nameof(PreLoadLevyMessage)} added for EmployerAccountId: {preLoadLevyRequestMessage.AccountId}");
                        return null;
                    }

                    logger.Info($"Added {nameof(PreLoadLevyMessage)} to queue: {QueueNames.LevyPreLoadRequest},  for EmployerAccountId: {preLoadLevyRequestMessage.AccountId}");

                    return await Task.FromResult(new PreLoadLevyMessage
                    {
                        EmployerAccountId = preLoadLevyRequestMessage.AccountId,
                        PeriodMonth = preLoadLevyRequestMessage.PeriodMonth,
                        PeriodYear = preLoadLevyRequestMessage.PeriodYear,
                    });
                });
        }
    }
}