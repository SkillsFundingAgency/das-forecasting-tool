using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Validation;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class PaymentEventValidatorFunction: IFunction
    {
        [FunctionName("PaymentEventValidatorFunction")]
        [return:Queue(QueueNames.PaymentProcessor)]
        public static PaymentCreatedMessage Run(
            [QueueTrigger(QueueNames.PaymentValidator)]PaymentCreatedMessage paymentCreatedMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return FunctionRunner.Run<PaymentEventValidatorFunction, PaymentCreatedMessage>(writer, executionContext, (container, logger) =>
                {
	                var telemetry = container.GetInstance<IAppInsightsTelemetry>();

					var validationResults = container.GetInstance<PaymentEventSuperficialValidator>()
                        .Validate(paymentCreatedMessage);

                    if (!validationResults.IsValid)
                    {
	                    telemetry.Warning("PaymentEventValidatorFunction", 
							$"Payment event failed superficial validation. Employer: {paymentCreatedMessage.EmployerAccountId} apprenticeship: {paymentCreatedMessage.ApprenticeshipId}, " +
							$"Errors:{validationResults.ToJson()}", 
							"FunctionRunner.Run", 
							executionContext.InvocationId);

						return null;
                    }

	                telemetry.Info("PaymentEventValidatorFunction", 
						$"Validated {nameof(PaymentCreatedMessage)} for EmployerAccountId: {paymentCreatedMessage.EmployerAccountId} fundingSource:{paymentCreatedMessage.FundingSource}", 
						"FunctionRunner.Run", 
						executionContext.InvocationId);
                    
                    return paymentCreatedMessage;
                });
        }
    }
}
