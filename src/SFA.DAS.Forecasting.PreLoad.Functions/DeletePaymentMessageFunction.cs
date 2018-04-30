using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Functions.Framework;

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
                    logger.Info($"{nameof(DeletePaymentMessageFunction)} started");

                    var dataService = container.GetInstance<PreLoadPaymentDataService>();

                    await dataService.DeletePayment(message.EmployerAccountId);
                    await dataService.DeleteEarningDetails(message.EmployerAccountId);

                    logger.Info($"{nameof(DeletePaymentMessageFunction)} finished.");
                });
        }
    }
}
