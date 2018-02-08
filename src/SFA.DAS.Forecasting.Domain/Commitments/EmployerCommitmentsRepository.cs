using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Domain.Events;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public class EmployerCommitmentsRepository
    {
        private readonly ICommitmentsDataService _dataService;
        private readonly IEventPublisher _eventPublisher;

        public EmployerCommitmentsRepository(ICommitmentsDataService dataService, IEventPublisher eventPublisher)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<EmployerCommitments> Get(long employerAccountId)
        {
            var commitments = await _dataService.GetCurrentCommitments(employerAccountId);
            return new EmployerCommitments(employerAccountId, commitments, _eventPublisher);
        }

        public async Task Store(EmployerCommitments commitments)
        {
            await _dataService.Store(commitments.Commitments);
        }
    }
}