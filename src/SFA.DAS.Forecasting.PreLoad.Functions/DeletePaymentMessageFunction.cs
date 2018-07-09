using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class DeletePaymentMessageFunction : IFunction
    {
        [FunctionName("DeletePaymentMessageFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.RemovePreLoadData)]PreLoadPaymentMessage message,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<DeletePaymentMessageFunction>(writer, executionContext,
                async (container, logger) =>
                {
	                var telemetry = container.GetInstance<IAppInsightsTelemetry>();

	                telemetry.Info("DeletePaymentMessageFunction", $"{nameof(DeletePaymentMessageFunction)} started for account: {message.HashedEmployerAccountId}", "FunctionRunner.Run", executionContext.InvocationId);

                    var dataService = container.GetInstance<PreLoadPaymentDataService>();
                    
                    await dataService.DeletePayment(message.EmployerAccountId);
                    await dataService.DeleteEarningDetails(message.EmployerAccountId);

	                telemetry.Info("DeletePaymentMessageFunction", $"{nameof(DeletePaymentMessageFunction)} finished for account: {message.HashedEmployerAccountId}.", "FunctionRunner.Run");
                });
        }
    }
}
