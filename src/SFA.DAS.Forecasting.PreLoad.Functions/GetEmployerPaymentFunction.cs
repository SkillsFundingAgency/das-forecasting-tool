using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class GetEmployerPaymentFunction
    {
        private readonly IEmployerDatabaseService _employerDatabaseService;
        private readonly IPreLoadPaymentDataService _preLoadPaymentDataService;

        public GetEmployerPaymentFunction(IEmployerDatabaseService employerDatabaseService, IPreLoadPaymentDataService preLoadPaymentDataService)
        {
            _employerDatabaseService = employerDatabaseService;
            _preLoadPaymentDataService = preLoadPaymentDataService;
        }
        [FunctionName("GetEmployerPaymentFunction")]
        [return: Queue(QueueNames.PreLoadEarningDetailsPayment)]
        public async Task<PreLoadPaymentMessage> Run(
            [QueueTrigger(QueueNames.PreLoadPayment)]PreLoadPaymentMessage message,
            [Queue(QueueNames.GenerateProjections)] ICollector<GenerateAccountProjectionCommand> outputQueueMessage,
            ILogger logger)
        {
            // Store all payments in TableStorage
            // Sends a message to CreateEarningRecord

           logger.LogInformation($"Storing data for EmployerAcount: {message.EmployerAccountId}");

           var payments = await _employerDatabaseService.GetEmployerPayments(message.EmployerAccountId, message.PeriodYear, message.PeriodMonth);

           if (!payments?.Any() ?? false)
           {
               logger.LogInformation($"No data found for {message.EmployerAccountId} add message to queue to build projection from last data set");
               outputQueueMessage.Add(new GenerateAccountProjectionCommand
               {
                   EmployerAccountId = message.EmployerAccountId,
                   ProjectionSource = ProjectionSource.PaymentPeriodEnd
               });
               return null;
           }

           foreach (var payment in payments)
           {
               await _preLoadPaymentDataService.StorePayment(payment);

               logger.LogInformation($"Stored new {nameof(payment)} for {payment.AccountId}");
           }

		   return message;
               
        }
    }
}
