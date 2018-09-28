using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Payments.Services;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class DeletePaymentMessageNoCommitmentFunction : IFunction
    {
        [FunctionName("DeletePaymentMessageNoCommitmentFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.RemovePreLoadData)]PreLoadPaymentMessage message,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<DeletePaymentMessageNoCommitmentFunction>(writer, executionContext,
                async (container, logger) =>
                {
                    logger.Info($"{nameof(DeletePaymentMessageNoCommitmentFunction)} started for account: {message.HashedEmployerAccountId}");
                    var dataService = container.GetInstance<PreLoadPaymentDataService>();
                    
                    await dataService.DeletePaymentNoCommitment(message.EmployerAccountId);
					await dataService.DeleteEarningDetailsNoCommitment(message.EmployerAccountId);

					logger.Info($"{nameof(DeletePaymentMessageNoCommitmentFunction)} finished for account: {message.HashedEmployerAccountId}.");
                });
        }
    }
}
