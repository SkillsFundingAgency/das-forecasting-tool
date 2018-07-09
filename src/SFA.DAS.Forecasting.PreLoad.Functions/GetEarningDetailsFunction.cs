using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class GetEarningDetailsFunction : IFunction
    {
        [FunctionName("GetEarningDetailsFunction")]
        [return: Queue(QueueNames.CreatePaymentMessage)]
        public static async Task<PreLoadPaymentMessage> Run(
            [QueueTrigger(QueueNames.PreLoadEarningDetailsPayment)]PreLoadPaymentMessage message, 
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<GetEarningDetailsFunction, PreLoadPaymentMessage>(writer, executionContext,
                async (container, logger) => {
	                var telemetry = container.GetInstance<IAppInsightsTelemetry>();

	                telemetry.Info("GetEarningDetailsFunction", $"Running {nameof(GetEarningDetailsFunction)} {message.EmployerAccountId}. {message.PeriodId}", "FunctionRunner.Run", executionContext.InvocationId);

					// Get ALL EarningDetails from Payment ProviderEventsAPI for a Employer and PeriodId

                    var paymentDataService = container.GetInstance<PaymentApiDataService>();
                    var hashingService = container.GetInstance<IHashingService>();
                    var dataService = container.GetInstance<PreLoadPaymentDataService>();

                    var earningDetails = await paymentDataService.PaymentForPeriod(message.PeriodId, message.EmployerAccountId);

                    var hashedAccountId = hashingService.HashValue(message.EmployerAccountId);
	                telemetry.Info("GetEarningDetailsFunction", $"Found {earningDetails.Count} for Account: {hashedAccountId}", "FunctionRunner.Run");

                    foreach (var item in earningDetails)
                    {
                        await dataService.StoreEarningDetails(message.EmployerAccountId, item);
                    }

	                telemetry.Info("GetEarningDetailsFunction", $"Sending message {nameof(message)} to {QueueNames.CreatePaymentMessage}", "FunctionRunner.Run");
                    return message;
                });
        }
    }
}
