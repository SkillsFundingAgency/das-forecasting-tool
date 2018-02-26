using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Domain.Commitments.Validation;
using SFA.DAS.Forecasting.Domain.Events;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public interface IEmployerCommitmentsRepository
    {
        Task<EmployerCommitments> Get(long employerAccountId);
        Task Store(EmployerCommitments commitments);
    }

    public class EmployerCommitmentsRepository : IEmployerCommitmentsRepository
    {
        private readonly ICommitmentsDataService _dataService;
        private readonly IEventPublisher _eventPublisher;
        private readonly CommitmentValidator _commitmentValidator;

        public EmployerCommitmentsRepository(
            ICommitmentsDataService dataService, 
            IEventPublisher eventPublisher,
            CommitmentValidator commitmentValidator)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _commitmentValidator = commitmentValidator;
        }

        public async Task<EmployerCommitments> Get(long employerAccountId)
        {
            var commitments = await _dataService.GetCurrentCommitments(employerAccountId);
            return new EmployerCommitments(employerAccountId, commitments, _eventPublisher, _commitmentValidator);
        }

        public async Task Store(EmployerCommitments commitments)
        {
            await _dataService.Store(commitments.Commitments);
        }
    }
}