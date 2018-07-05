using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Levy.Handlers;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationEventStoreFunction : IFunction
    {
        [FunctionName("LevyDeclarationEventStoreFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.StoreLevyDeclaration)]LevySchemeDeclarationUpdatedMessage levySchemeUpdatedMessage, ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<LevyDeclarationEventStoreFunction>(writer, executionContext,
                async (container, logger) =>
                {
	                var telemetry = container.GetInstance<IAppInsightsTelemetry>();

					telemetry.Debug("LevyDeclarationEventStoreFunction", "Getting levy declaration handler from container.", "FunctionRunner.Run", executionContext.InvocationId);
                    var handler = container.GetInstance<StoreLevyDeclarationHandler>();
	                if (handler == null)
	                {
		                telemetry.Error("LevyDeclarationEventStoreFunction",
			                new InvalidOperationException($"Failed to get levy handler from container."),
			                "Failed to get levy handler from container.",
							"FunctionRunner.Run");
		                throw new InvalidOperationException($"Failed to get levy handler from container.");
	                }
	                await handler.Handle(levySchemeUpdatedMessage, QueueNames.AllowProjection);

					telemetry.Info("LevyDeclarationEventStoreFunction", $"Finished handling levy declaration for EmployerAccountId: {levySchemeUpdatedMessage.AccountId}, PayrollYear: {levySchemeUpdatedMessage.PayrollYear}, month: {levySchemeUpdatedMessage.PayrollMonth}, scheme: {levySchemeUpdatedMessage.EmpRef}", "FunctionRunner.Run", executionContext.InvocationId);
                });
        }
    }
}
