using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentPreLoadAddEarningDetailsFunction : IFunction
    {
        [FunctionName("PaymentPreLoadAddEarningDetailsFunction")]
        [return: Queue(QueueNames.PaymentValidator)]
        public static async Task<PaymentCreatedMessage> Run(
            [QueueTrigger(QueueNames.AddEarningDetails, Connection = "")]PaymentCreatedMessage message, 
            TraceWriter writer)
        {
            return await FunctionRunner.Run<PaymentPreLoadAddEarningDetailsFunction, PaymentCreatedMessage>(writer,
                async (container, logger) =>
                {
                    var paymentDataService = container.GetInstance<EmployerDataService>();

                    var earningDetails = await paymentDataService.PaymentForPeriod(message.CollectionPeriod, message.EmployerAccountId);
                    message.EarningDetails = earningDetails;
                    logger.Info($"Added EarningDetails to {nameof(PaymentCreatedMessage)} for {message.EmployerAccountId} ");
                    return message;
                });
        }
    }
}
