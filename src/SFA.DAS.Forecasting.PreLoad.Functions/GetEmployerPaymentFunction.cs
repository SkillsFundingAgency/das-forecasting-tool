using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class GetEmployerPaymentFunction : IFunction
    {
        [FunctionName("GetEmployerPaymentFunction")]
        [return: Queue(QueueNames.PreLoadEarningDetailsPayment)]
        public static async Task<PreLoadPaymentMessage> Run(
            [QueueTrigger(QueueNames.PreLoadPayment)]PreLoadPaymentMessage message,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            // Store all payments in TableStorage
            // Sends a message to CreateEarningRecord

            return await FunctionRunner.Run<GetEmployerPaymentFunction, PreLoadPaymentMessage>(writer, executionContext,
               async (container, logger) =>
               {
	               var telemetry = container.GetInstance<IAppInsightsTelemetry>();

				   var employerData = container.GetInstance<IEmployerDatabaseService>();

				   telemetry.Info("GetEmployerPaymentFunction", $"Storing data for EmployerAcount: {message.EmployerAccountId}", "FunctionRunner.Run", executionContext.InvocationId);

				   var payments = await employerData.GetEmployerPayments(message.EmployerAccountId, message.PeriodYear, message.PeriodMonth);

                   if (!payments?.Any() ?? false)
                   {
	                   telemetry.Info("GetEmployerPaymentFunction", $"No data found for {message.EmployerAccountId}", "FunctionRunner.Run");

                       return null;
                   }

                   foreach (var payment in payments)
                   {
                       var dataService = container.GetInstance<PreLoadPaymentDataService>();
                       await dataService.StorePayment(payment);

	                   telemetry.Info("GetEmployerPaymentFunction", $"Stored new {nameof(payment)} for {payment.AccountId}", "FunctionRunner.Run");
                   }

                   return message;
               });
        }
    }
}
