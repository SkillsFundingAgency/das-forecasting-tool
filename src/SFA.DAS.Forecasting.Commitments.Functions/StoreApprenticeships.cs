using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.Commitments.Handlers;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    public class StoreApprenticeships 
    {
        private readonly IStoreCommitmentHandler _handler;

        public StoreApprenticeships(IStoreCommitmentHandler handler)
        {
            _handler = handler;
        }
        [FunctionName("StoreApprenticeships")]
        public async Task Run([QueueTrigger(QueueNames.StoreApprenticeships)]ApprenticeshipMessage message, ILogger logger)
        {
            logger.LogDebug($"Storing apprenticeship. Account: {message.EmployerAccountId}, apprenticeship id: {message.ApprenticeshipId}");
            message.ProjectionSource = ProjectionSource.Commitment;
            await _handler.Handle(message,QueueNames.AllowProjection);
            logger.LogInformation($"Stored commitment. Apprenticeship id: {message.ApprenticeshipId}");
        }
    }
}
