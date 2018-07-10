using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Domain.Shared;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Domain.Estimations
{
    public interface IAccountEstimationProjectionRepository
    {
        Task<IAccountEstimationProjection> Get(AccountEstimation accountEstimation);
    }

    public class AccountEstimationProjectionRepository : IAccountEstimationProjectionRepository
    {
        private readonly IAccountProjectionDataSession _accountProjectionRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly ICommitmentModelListBuilder _commitmentModelListBuilder;
        private readonly ICurrentBalanceRepository _currentBalanceRepository;

        public AccountEstimationProjectionRepository(ICurrentBalanceRepository currentBalanceRepository,
            IAccountProjectionDataSession accountProjectionRepository, IDateTimeService dateTimeService,
            ICommitmentModelListBuilder commitmentModelListBuilder)
        {
            _accountProjectionRepository = accountProjectionRepository;
            _dateTimeService = dateTimeService;
            _commitmentModelListBuilder = commitmentModelListBuilder ?? throw new ArgumentNullException(nameof(commitmentModelListBuilder));
            _currentBalanceRepository = currentBalanceRepository ?? throw new ArgumentNullException(nameof(currentBalanceRepository));
        }

        public async Task<IAccountEstimationProjection> Get(AccountEstimation accountEstimation)
        {
            var balance = await _currentBalanceRepository.Get(accountEstimation.EmployerAccountId);
            var commitments = _commitmentModelListBuilder.Build(accountEstimation.EmployerAccountId, accountEstimation.VirtualApprenticeships);

            var actualProjection = await _accountProjectionRepository.Get(accountEstimation.EmployerAccountId);

            var employerCommitmentsModel = new EmployerCommitmentsModel();
            employerCommitmentsModel.SendingEmployerTransferCommitments = commitments;

            var employerCommitments = new EmployerCommitments(accountEstimation.EmployerAccountId, employerCommitmentsModel);
            var accountEstimationProjectionCommitments = new AccountEstimationProjectionCommitments(employerCommitments, actualProjection.Projections);

            return new AccountEstimationProjection(new Account(accountEstimation.EmployerAccountId, balance.Amount, 0, balance.TransferAllowance, balance.RemainingTransferBalance), accountEstimationProjectionCommitments, _dateTimeService);
        }
    }
}