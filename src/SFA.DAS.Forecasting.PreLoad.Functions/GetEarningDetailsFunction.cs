using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Application.Shared.Services;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class GetEarningDetailsFunction 
    {
        private readonly IPaymentApiDataService _paymentApiDataService;
        private readonly IPreLoadPaymentDataService _preLoadPaymentDataService;

        public GetEarningDetailsFunction(IPaymentApiDataService paymentApiDataService, IPreLoadPaymentDataService preLoadPaymentDataService)
        {
            _paymentApiDataService = paymentApiDataService;
            _preLoadPaymentDataService = preLoadPaymentDataService;
        }
        [FunctionName("GetEarningDetailsFunction")]
        [return: Queue(QueueNames.CreatePaymentMessage)]
        public async Task<PreLoadPaymentMessage> Run(
            [QueueTrigger(QueueNames.PreLoadEarningDetailsPayment)]PreLoadPaymentMessage message, ILogger logger)
        {
            // Get ALL EarningDetails from Payment ProviderEventsAPI for a Employer and PeriodId
            logger.LogInformation($"Running {nameof(GetEarningDetailsFunction)} {message.EmployerAccountId}. {message.PeriodId}");

		    var earningDetails = await _paymentApiDataService.PaymentForPeriod(message.PeriodId, message.EmployerAccountId);
            
            logger.LogInformation($"Found {earningDetails.Count} for Account: {message.EmployerAccountId}");

            foreach (var item in earningDetails)
            {
                await _preLoadPaymentDataService.StoreEarningDetails(message.EmployerAccountId, item);
            }
            
		    logger.LogInformation($"Sending message {nameof(message)} to {QueueNames.CreatePaymentMessage}");
            return message;
        }
    }
}
