using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class AllEmployerPaymentPreLoadFunction : IFunction
    {
        [FunctionName("AllEmployerPaymentPreloadFunction")]
        public static async Task Run([QueueTrigger(QueueNames.PreLoadAllPaymentRequest)]string message,
            [Queue(QueueNames.PreLoadPayment)] ICollector<PreLoadPaymentMessage> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {

            await FunctionRunner.Run<AllEmployerPaymentPreLoadFunction>(writer, executionContext,
                async (container, logger) =>
                {
                    var employerData = container.GetInstance<IEmployerDatabaseService>();
                    var periodEnds = await employerData.GetPeriodIds();
                    var periodInformation = periodEnds.OrderByDescending(c => c.PeriodEndId).FirstOrDefault();

                    if (periodInformation == null)
                    {
                        logger.Info("No payment period information found");
                        return;
                    }

                    var req = new AllEmployersPreLoadPaymentRequest
                    {
                        PeriodId = periodInformation.PeriodEndId,
                        PeriodYear = periodInformation.CalendarPeriodYear,
                        PeriodMonth = periodInformation.CalendarPeriodMonth
                    };

                    await AllEmployersPaymentDataLoader.QueueEmployersWithNewPayments(req, outputQueueMessage, logger, container);
                });
        }
    }
}