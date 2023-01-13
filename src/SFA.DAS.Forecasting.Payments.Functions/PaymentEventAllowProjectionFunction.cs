using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Payments.Handlers;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentEventAllowProjectionFunction 
    {
        private readonly IAllowAccountProjectionsHandler _handler;

        public PaymentEventAllowProjectionFunction(IAllowAccountProjectionsHandler handler)
        {
            _handler = handler;
        }
        [FunctionName("PaymentEventAllowProjectionFunction")]
        public async Task Run(
            [QueueTrigger(QueueNames.AllowProjection)]PaymentCreatedMessage paymentCreatedMessage,
            [Queue(QueueNames.GenerateProjections)] ICollector<GenerateAccountProjectionCommand> collector,
            ILogger logger)
        {
            logger.LogInformation("Getting payment declaration handler from container.");
            
            var allowedEmployerAccounts = (await _handler.AllowedEmployerAccountIds(paymentCreatedMessage)).ToList();

            if (allowedEmployerAccounts.Any())
            {
                foreach (var allowedEmployerAccount in allowedEmployerAccounts)
                {
	                logger.LogInformation($"Now sending message to trigger the account projections for employer '{allowedEmployerAccount}', period: {paymentCreatedMessage.CollectionPeriod?.Id}, {paymentCreatedMessage.CollectionPeriod?.Month}");

                    var projectionSource = paymentCreatedMessage.ProjectionSource == ProjectionSource.Commitment 
                                            ? paymentCreatedMessage.ProjectionSource : ProjectionSource.PaymentPeriodEnd;

                    collector.Add(new GenerateAccountProjectionCommand
	                {
		                EmployerAccountId = allowedEmployerAccount,
		                ProjectionSource = projectionSource,
	                });
                }
            }
            else
            {
                logger.LogInformation($"Cannot generate the projections, still handling payment events. Employer: {paymentCreatedMessage.EmployerAccountId}");
			}
        }
    }
}
