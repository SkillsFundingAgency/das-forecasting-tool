using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentPreLoadGetEarningDetailsFunction : IFunction
    {
        [FunctionName("PaymentPreLoadGetEarningDetailsFunction")]
        [return: Queue(QueueNames.AddEarningDetails)]
        public static async Task<PreLoadMessage> Run(
            [QueueTrigger(QueueNames.PreLoadEarningDetailsPayment)]PreLoadMessage message, 
            TraceWriter writer)
        {
            return await FunctionRunner.Run<PaymentPreLoadGetEarningDetailsFunction, PreLoadMessage>(writer,
                async (container, logger) => {

                    // Get ALL EarningDetails from Payment ProviderEventsAPI for a Employer and PeriodId
                    //  --> Save to TS
                    logger.Info($"Running {nameof(PaymentPreLoadGetEarningDetailsFunction)} {message.EmployerAccountId}. {message.PeriodId}");

                    var paymentDataService = container.GetInstance<PaymentApiDataService>();
                    var hashingService = container.GetInstance<IHashingService>();
                    var dataService = container.GetInstance<PreLoadPaymentDataService>();

                    var hashedAccountId = hashingService.HashValue(message.EmployerAccountId);
                    var earningDetails = await paymentDataService.PaymentForPeriod(message.PeriodId, hashedAccountId);

                    foreach (var item in earningDetails)
                    {
                        dataService.StoreEarningDetails(message.EmployerAccountId, item);
                    }
                    // When done send a msg to 
                    return message;
                });
        }
    }
}
