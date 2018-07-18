using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Levy.Validation;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Levy.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationEventValidatorFunction: IFunction
    {
        [FunctionName("LevyDeclarationEventValidatorFunction")]
        [return:Queue(QueueNames.StoreLevyDeclaration)]
        public static LevySchemeDeclarationUpdatedMessage Run(
            [QueueTrigger(QueueNames.ValidateDeclaration)]LevySchemeDeclarationUpdatedMessage message, 
            TraceWriter writer, ExecutionContext executionContext)
        {
            return FunctionRunner.Run<LevyDeclarationEventValidatorFunction, LevySchemeDeclarationUpdatedMessage>(writer, executionContext,
                (container, logger) =>
                {
					var telemetry = container.GetInstance<IAppInsightsTelemetry>();

                    var validationResults = container.GetInstance<LevyDeclarationEventValidator>()
                        .Validate(message);
                    if (!validationResults.IsValid)
                    {
	                    telemetry.Warning("LevyDeclarationEventValidatorFunction", $"Levy declaration event failed superficial validation. Employer id: {message.AccountId}, Period: {message.PayrollMonth}, {message.PayrollYear}, Scheme: {message.EmpRef}.", "FunctionRunner.Run", executionContext.InvocationId);

                        return null;
                    }

	                telemetry.Info("LevyDeclarationEventValidatorFunction", $"Validated {nameof(LevySchemeDeclarationUpdatedMessage)} for EmployerAccountId: {message.AccountId}", "FunctionRunner.Run", executionContext.InvocationId);

                    return  message;
                });
        }
    }
}
