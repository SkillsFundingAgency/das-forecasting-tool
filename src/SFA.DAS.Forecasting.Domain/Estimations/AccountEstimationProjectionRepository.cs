using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Commitments.Validation;
using SFA.DAS.Forecasting.Domain.Events;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.Estimations
{
    public interface IAccountEstimationProjectionRepository
    {
        Task<IAccountEstimationProjection> Get(long accountId);
        Task<IAccountEstimationProjection> Get(AccountEstimation accountEstimation);
    }

    public class AccountEstimationProjectionRepository: IAccountEstimationProjectionRepository
    {
        private readonly IAccountEstimationRepository _accountEstimationRepository;
        private readonly ICurrentBalanceRepository _currentBalanceRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICommitmentValidator _commitmentsValidator;

        public AccountEstimationProjectionRepository(IAccountEstimationRepository accountEstimationRepository,
            ICurrentBalanceRepository currentBalanceRepository, IEventPublisher eventPublisher, ICommitmentValidator commitmentsValidator)
        {
            _accountEstimationRepository = accountEstimationRepository ?? throw new ArgumentNullException(nameof(accountEstimationRepository));
            _currentBalanceRepository = currentBalanceRepository ?? throw new ArgumentNullException(nameof(currentBalanceRepository));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _commitmentsValidator = commitmentsValidator ?? throw new ArgumentNullException(nameof(commitmentsValidator));
        }

        public async Task<IAccountEstimationProjection> Get(long accountId)
        {
            var accountEstimation = await _accountEstimationRepository.Get(accountId);
            return await Get(accountEstimation);
        }

        public async Task<IAccountEstimationProjection> Get(AccountEstimation accountEstimation)
        {
            var balance = await _currentBalanceRepository.Get(accountEstimation.EmployerAccountId);
            var employerCommitments = new EmployerCommitments(accountEstimation.EmployerAccountId, new List<Commitment>(), _eventPublisher, _commitmentsValidator);
            //TODO: convert virtual apprenticeships to list of Employer Commitments.
            return new AccountEstimationProjection(new Account(accountEstimation.EmployerAccountId, balance.Amount, 0, balance.TransferAllowance, balance.RemainingTransferBalance), employerCommitments);
        }
    }
}