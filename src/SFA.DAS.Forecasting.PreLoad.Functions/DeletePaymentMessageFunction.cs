using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Payments.Services;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class DeletePaymentMessageFunction 
    {
        private readonly IPreLoadPaymentDataService _preLoadPaymentDataService;

        public DeletePaymentMessageFunction(IPreLoadPaymentDataService preLoadPaymentDataService)
        {
            _preLoadPaymentDataService = preLoadPaymentDataService;
        }
            
        [FunctionName("DeletePaymentMessageFunction")]
        public async Task Run(
            [QueueTrigger(QueueNames.RemovePreLoadData)]PreLoadPaymentMessage message, ILogger logger)
        {
            logger.LogInformation($"{nameof(DeletePaymentMessageFunction)} started for account: {message.EmployerAccountId}");
            
            await _preLoadPaymentDataService.DeletePayment(message.EmployerAccountId);
			await _preLoadPaymentDataService.DeleteEarningDetails(message.EmployerAccountId);

			logger.LogInformation($"{nameof(DeletePaymentMessageFunction)} finished for account: {message.EmployerAccountId}.");
            
        }
    }
}
