using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Payments.Functions.PreLoad
{
    public class GetEarningDetailsFunction : IFunction
    {
        [FunctionName("GetEarningDetailsFunction")]
        [return: Queue(QueueNames.AddEarningDetails)]
        public static async Task<PreLoadPaymentMessage> Run(
            [QueueTrigger(QueueNames.PreLoadEarningDetailsPayment)]PreLoadPaymentMessage message, 
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<GetEarningDetailsFunction, PreLoadPaymentMessage>(writer, executionContext,
                async (container, logger) => {

                    // Get ALL EarningDetails from Payment ProviderEventsAPI for a Employer and PeriodId
                    //  --> Save to TS
                    logger.Info($"Running {nameof(GetEarningDetailsFunction)} {message.EmployerAccountId}. {message.PeriodId}");

                    var paymentDataService = container.GetInstance<PaymentApiDataService>();
                    var hashingService = container.GetInstance<IHashingService>();
                    var dataService = container.GetInstance<PreLoadPaymentDataService>();

                    var hashedAccountId = hashingService.HashValue(message.EmployerAccountId);
                    var earningDetails = await paymentDataService.PaymentForPeriod(message.PeriodId, hashedAccountId);

                    foreach (var item in earningDetails)
                    {
                        await dataService.StoreEarningDetails(message.EmployerAccountId, item);
                    }

                    logger.Info($"Sending message {nameof(message)} to {QueueNames.AddEarningDetails}");
                    return message;
                });
        }
    }
}
