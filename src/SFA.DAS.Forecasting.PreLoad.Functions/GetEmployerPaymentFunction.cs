using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
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
                   var employerData = container.GetInstance<IEmployerDatabaseService>();
                   logger.Info($"Storing data for EmployerAcount: {message.EmployerAccountId}");

                   var payments = await employerData.GetEmployerPayments(message.EmployerAccountId, message.PeriodYear, message.PeriodMonth);

                   if (!payments?.Any() ?? false)
                   {
                       logger.Info($"No data found for {message.EmployerAccountId}");
                       return null;
                   }

                   foreach (var payment in payments)
                   {
                       var dataService = container.GetInstance<PreLoadPaymentDataService>();
                       await dataService.StorePayment(payment);

                       logger.Info($"Stored new {nameof(payment)} for {payment.AccountId}");
                   }

                   return message;
               });
        }
    }
}
