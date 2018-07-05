using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Levy.Handlers;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationAllowProjectionFunction : IFunction
    {
        [FunctionName("LevyDeclarationAllowProjectionFunction")]
        [return: Queue(QueueNames.GenerateProjections)]
        public static async Task<GenerateAccountProjectionCommand> Run(
            [QueueTrigger(QueueNames.AllowProjection)]LevySchemeDeclarationUpdatedMessage message,
            TraceWriter writer, ExecutionContext executionContext)
        {
            return await FunctionRunner.Run<LevyDeclarationAllowProjectionFunction, GenerateAccountProjectionCommand>(writer, executionContext,
                async (container, logger) =>
                {
	                var telemetry = container.GetInstance<IAppInsightsTelemetry>();

					telemetry.Info("LevyDeclarationAllowProjectionFunction", "Getting levy declaration handler from container.", "FunctionRunner.Run", executionContext.InvocationId);

                    var handler = container.GetInstance<AllowAccountProjectionsHandler>();
	                if (handler == null)
	                {
						telemetry.Error("LevyDeclarationAllowProjectionFunction", new InvalidOperationException("Faild to get levy handler from container."), "Getting levy declaration handler from container.", "FunctionRunner.Run");
		                throw new InvalidOperationException("Faild to get levy handler from container.");
					}
	                    
                    var allowProjections = await handler.Allow(message);
                    if (!allowProjections)
                    {
	                    telemetry.Warning("LevyDeclarationAllowProjectionFunction", $"Cannot generate the projections, still handling levy declarations. Employer: {message.AccountId}", "FunctionRunner.Run", executionContext.InvocationId);

                        return null;
                    }

	                telemetry.Info("LevyDeclarationAllowProjectionFunction", $"Now sending message to trigger the account projections for employer '{message.AccountId}'", "FunctionRunner.Run", executionContext.InvocationId);

                    return new GenerateAccountProjectionCommand
                    {
                        EmployerAccountId = message.AccountId,
                        ProjectionSource = ProjectionSource.LevyDeclaration
                    };
                });
        }
    }
}