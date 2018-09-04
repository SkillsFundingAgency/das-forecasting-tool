using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Apprenticeship.Functions
{
    public class GetApprenticeshipHttpTrigger : IFunction
    {
        [FunctionName("GetApprenticeshipHttpTrigger")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function, 
            "get", Route = "GetApprenticeshipHttpTrigger")]HttpRequestMessage req,
            [Queue(QueueNames.GetApprenticeshipsForEmployer)] ICollector<GetApprenticesihpMessage> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<GetApprenticeshipHttpTrigger, string>(writer, executionContext,
               async (container, logger) =>
               {
                   logger.Debug("Getting all employer ids...");
                   var employerCommitmentsApi = container.GetInstance<IEmployerCommitmentApi>();
                   var allEmployerIds = await employerCommitmentsApi.GetAllEmployerAccountIds();

                   var count = 0;
                   foreach (var id in allEmployerIds)
                   {
                       count++;
                       outputQueueMessage.Add(new GetApprenticesihpMessage
                       {
                           EmployerId = id
                       });
                   }
                   var msg = $"Added {count} employer id messages to the queue {nameof(QueueNames.GetApprenticeshipsForEmployer)}.";
                   logger.Info(msg);
                   return msg;
               });
        }
    }
}
