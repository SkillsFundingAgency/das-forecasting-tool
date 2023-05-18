using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Messages;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventStorePaymentFunction
    {
        private readonly IProcessEmployerPaymentHandler _handler;

        public PaymentEventStorePaymentFunction(IProcessEmployerPaymentHandler handler)
        {
            _handler = handler;
        }
        [FunctionName("PaymentEventStorePaymentFunction")]
        [return: Queue(QueueNames.CommitmentProcessor)]
        public async Task<PaymentCreatedMessage> Run(
            [QueueTrigger(QueueNames.PaymentProcessor)]PaymentCreatedMessage paymentCreatedMessage,
            ILogger logger)
        {
            logger.LogDebug($"Storing the payment.  Employer: {paymentCreatedMessage.EmployerAccountId}, apprenticeship: {paymentCreatedMessage.ApprenticeshipId}.");
            
            await _handler.Handle(paymentCreatedMessage, QueueNames.AllowProjection);
            logger.LogInformation($"Finished storing payment. Employer: {paymentCreatedMessage.EmployerAccountId}, apprenticeship: {paymentCreatedMessage.ApprenticeshipId}.");
            return paymentCreatedMessage;
            
        }
    }
}
