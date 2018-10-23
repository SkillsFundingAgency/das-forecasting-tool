using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class PaymentPreLoadServiceBusFunction : IFunction
    {
        [FunctionName("PaymentPreLoadServiceBusFunction")]
        [return: Queue(QueueNames.PreLoadPayment)]
        public static async Task<PreLoadPaymentMessage> Run(
            [ServiceBusTrigger("PaymentPeriod", "Forecasting", AccessRights.Listen)]PreLoadPaymentRequestMessage preLoadPaymentRequestMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<PaymentPreLoadServiceBusFunction, PreLoadPaymentMessage>(writer, executionContext,
                async (container, logger) =>
                {
                    if (!preLoadPaymentRequestMessage.PaymentsProcessed)
                    {
                        logger.Info($"No new messages of type {nameof(PreLoadPaymentMessage)} added for EmployerAccountId: {preLoadPaymentRequestMessage.AccountId}");
                        return null;
                    }

                    logger.Info($"Added {nameof(PreLoadPaymentMessage)} to queue: {QueueNames.PreLoadPayment},  for EmployerAccountId: {preLoadPaymentRequestMessage.AccountId}");
                    
                    return await Task.FromResult(new PreLoadPaymentMessage
                    {
                        EmployerAccountId = preLoadPaymentRequestMessage.AccountId,
                        PeriodId = preLoadPaymentRequestMessage.PayrollPeriod
                    });
                });
        }
    }
}
