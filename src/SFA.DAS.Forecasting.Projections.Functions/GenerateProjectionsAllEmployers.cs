using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Levy.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Projections.Functions
{
    public class GenerateProjectionsAllEmployers : IFunction
    {
        [FunctionName("GenerateProjectionsAllEmployers")]
        public static async Task Run(
            [QueueTrigger(QueueNames.GenerateProjectionForAllAccounts)]ProjectionSource message,
            [Queue(QueueNames.GenerateProjections)] ICollector<GenerateAccountProjectionCommand> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {

            await FunctionRunner.Run<GenerateProjectionsAllEmployers>(writer, executionContext,
                async (container, logger) =>
                {
                    var levyDataService = container.GetInstance<IEmployerDataService>();

                    var employerIds = await levyDataService.GetAllAccounts();

                    logger.Info($"Found {employerIds.Count} employer(s) for period for Projection Type {message}");

                    foreach (var id in employerIds)
                    {
                        outputQueueMessage.Add(new GenerateAccountProjectionCommand
                        {
                            EmployerAccountId = id,
                            ProjectionSource = message
                        });
                    }

                    logger.Info($"Created {employerIds.Count} {nameof(PreLoadLevyMessage)} messages");
                });
        }
    }
}