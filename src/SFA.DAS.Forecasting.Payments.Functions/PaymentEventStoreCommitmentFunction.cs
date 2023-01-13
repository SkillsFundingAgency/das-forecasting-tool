using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Commitments.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Messages;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventStoreCommitmentFunction
    {
        private readonly IStoreCommitmentHandler _handler;

        public PaymentEventStoreCommitmentFunction(IStoreCommitmentHandler handler)
        {
            _handler = handler;
        }
		[FunctionName("PaymentEventStoreCommitmentFunction")]
		public async Task Run(
            [QueueTrigger(QueueNames.CommitmentProcessor)]PaymentCreatedMessage paymentCreatedMessage,
            ILogger logger)
        {
            logger.LogDebug($"Storing commitment. Account: {paymentCreatedMessage.EmployerAccountId}, apprenticeship id: {paymentCreatedMessage.ApprenticeshipId}");
	        
			await _handler.Handle(paymentCreatedMessage, QueueNames.AllowProjection);
            logger.LogInformation($"Stored commitment. Apprenticeship id: {paymentCreatedMessage.ApprenticeshipId}");
        }
    }
}
