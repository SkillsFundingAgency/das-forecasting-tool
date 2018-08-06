using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Domain.Commitments.Validation;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public interface IEmployerCommitmentRepository
    {
        Task<EmployerCommitment> Get(long employerAccountId, long apprenticeshipId);
        Task Store(EmployerCommitment commitment);
    }

    public class EmployerCommitmentRepository : IEmployerCommitmentRepository
    {
        private readonly ICommitmentValidator _commitmentValidator;
        private readonly ICommitmentsDataService _dataService;

        public EmployerCommitmentRepository(ICommitmentValidator commitmentValidator, ICommitmentsDataService dataService)
        {
            _commitmentValidator = commitmentValidator ?? throw new ArgumentNullException(nameof(commitmentValidator));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }

        public async Task<EmployerCommitment> Get(long employerAccountId, long apprenticeshipId)
        {
            var commitment = await _dataService.Get(employerAccountId, apprenticeshipId)
                             ?? new CommitmentModel
                             {
                                 EmployerAccountId = employerAccountId,
                                 ApprenticeshipId = apprenticeshipId
                             };
            return new EmployerCommitment(commitment, _commitmentValidator);
        }

        public async Task Store(EmployerCommitment commitment)
        {
            await _dataService.Store(commitment.Commitment);
        }
    }
}