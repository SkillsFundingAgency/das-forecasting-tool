using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Messages;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventNoCommitmentStorePaymentFunction
    {
        private readonly IProcessEmployerPaymentHandler _handler;

        public PaymentEventNoCommitmentStorePaymentFunction(IProcessEmployerPaymentHandler handler)
        {
            _handler = handler;
        }
        [FunctionName("PaymentEventNoCommitmentStorePaymentFunction")]
        public async Task Run(
            [QueueTrigger(QueueNames.PaymentNoCommitmentProcessor)]PaymentCreatedMessage paymentCreatedMessage,
            ILogger logger)
        {
            logger.LogDebug($"Storing the payment.  Employer: {paymentCreatedMessage.EmployerAccountId}, apprenticeship: {paymentCreatedMessage.ApprenticeshipId}.");
            
            await _handler.Handle(paymentCreatedMessage, string.Empty);
            logger.LogInformation($"Finished storing payment. Employer: {paymentCreatedMessage.EmployerAccountId}, apprenticeship: {paymentCreatedMessage.ApprenticeshipId}.");
            
        }
    }
}
