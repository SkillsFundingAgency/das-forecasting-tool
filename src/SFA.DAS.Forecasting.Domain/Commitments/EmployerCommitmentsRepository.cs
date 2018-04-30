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
    }

    public class EmployerCommitmentsRepository : IEmployerCommitmentsRepository
    {
        private readonly ICommitmentsDataService _dataService;

        public EmployerCommitmentsRepository(
            ICommitmentsDataService dataService, 
            IEventPublisher eventPublisher,
            CommitmentValidator commitmentValidator)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }

        public async Task<EmployerCommitments> Get(long employerAccountId)
        {
            var commitments = await _dataService.GetCurrentCommitments(employerAccountId);
            return new EmployerCommitments(employerAccountId, commitments);
        }
    }
}