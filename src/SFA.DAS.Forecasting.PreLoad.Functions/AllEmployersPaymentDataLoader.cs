using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class AllEmployersPaymentDataLoader
    {
        public static async Task<string> QueueEmployersWithNewPayments(AllEmployersPreLoadPaymentRequest preLoadRequest, ICollector<PreLoadPaymentMessage> outputQueueMessage, ILog logger, IContainer container)
        {
            var employerData = container.GetInstance<IEmployerDatabaseService>();
            var hashingService = container.GetInstance<IHashingService>();

            var employerIds = await employerData.GetAccountIds();

            var messageCount = 0;
            foreach (var accountId in employerIds)
            {
                messageCount++;
                var hashedAccountId = hashingService.HashValue(accountId);
                outputQueueMessage.Add(
                    new PreLoadPaymentMessage
                    {
                        EmployerAccountId = accountId,
                        HashedEmployerAccountId = hashedAccountId,
                        PeriodYear = preLoadRequest.PeriodYear,
                        PeriodMonth = preLoadRequest.PeriodMonth,
                        PeriodId = preLoadRequest.PeriodId,
                        SubstitutionId = null
                    }
                );
            }

            var msg =
                $"Added {messageCount} message(s) to queue for year: {preLoadRequest.PeriodYear} and month: {preLoadRequest.PeriodMonth}";
            logger.Info(msg);
            return msg;
        }
    }
}