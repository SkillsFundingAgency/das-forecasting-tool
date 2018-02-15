using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentPreLoadStorePaymentMessageFunction : IFunction
    {
        [FunctionName("PaymentPreLoadStorePaymentMessageFunction")]
        [return: Queue(QueueNames.PreLoadEarningDetailsPayment)]
        public static async Task<PreLoadMessage> Run(
            [QueueTrigger(QueueNames.PreLoadPayment)]PreLoadPaymentMessage message,
            TraceWriter writer)
        {
            // Store all payments in TableStorage
            // Sends a message to CreateEarningRecord

            return await FunctionRunner.Run<PaymentPreLoadStorePaymentMessageFunction, PreLoadMessage>(writer,
               async (container, logger) =>
               {
                   var employerData = container.GetInstance<IEmployerDatabaseService>();

                   var payments = await employerData.GetEmployerPayments(message.EmployerAccountId, message.PeriodYear, message.PeriodMonth);

                   if (payments == null || !payments.Any())
                   {
                       logger.Info($"No data found for {message.EmployerAccountId}");
                   }

                   foreach (var payment in payments)
                   {
                       var dataService = container.GetInstance<PreLoadPaymentDataService>();
                       await dataService.StorePayment(payment);

                       logger.Info($"Stored new {nameof(payment)} for {payment.AccountId}");
                   }

                   return
                       new PreLoadMessage
                       {
                           EmployerAccountId = message.EmployerAccountId,
                           PeriodId = message.PeriodId
                       };
               });
        }
    }
}
