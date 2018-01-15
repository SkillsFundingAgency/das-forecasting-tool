using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Payments.Messages.Events;
using SFA.DAS.Messaging.POC;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.PaymentsAdapter.Functions
{
    [StorageAccount("StorageConnectionString")]
    public static class PublishPaymentEventFunction
    {
        [FunctionName("PublishPaymentEventFunction")]
        public static void Run(
            [QueueTrigger(QueueNames.PublishPaymentEvent)]PaymentEvent payment,
            TraceWriter traceWriter)
        {
            traceWriter.Info($"Now publishing payment: {payment.Id}, employer: {payment.EmployerAccountId}.");
            var publisher = Ioc.Container.GetInstance<IMessagePublisher>();
            publisher.Publish(payment);
            traceWriter.Info($"Finished publishing payment data. {payment.Id}, employer: {payment.EmployerAccountId}");
        }
    }
}