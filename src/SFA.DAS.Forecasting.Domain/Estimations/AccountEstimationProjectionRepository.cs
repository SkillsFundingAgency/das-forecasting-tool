﻿using System;
using System.Linq;
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
        Task<IAccountEstimationProjection> Get(AccountEstimation accountEstimation, bool showExpiredFunds = false);
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

        public async Task<IAccountEstimationProjection> Get(AccountEstimation accountEstimation, bool showExpiredFunds = false)
        {
            var balance = await _currentBalanceRepository.Get(accountEstimation.EmployerAccountId);
            var commitments = _commitmentModelListBuilder.Build(accountEstimation.EmployerAccountId, accountEstimation.Apprenticeships);

            var actualProjections = await _accountProjectionRepository.Get(accountEstimation.EmployerAccountId);

            var employerCommitmentsModel = new EmployerCommitmentsModel
            {
                SendingEmployerTransferCommitments = commitments
                    .Where(m => m.FundingSource == Models.Payments.FundingSource.Transfer || m.FundingSource == 0)
                    .ToList(),
                LevyFundedCommitments = commitments
                    .Where(m => m.FundingSource == Models.Payments.FundingSource.Levy)
                    .ToList()
            };

            var levyFundsIn = actualProjections?.FirstOrDefault()?.LevyFundsIn ?? 0;

            var employerCommitments = new EmployerCommitments(accountEstimation.EmployerAccountId, employerCommitmentsModel);
            var accountEstimationProjectionCommitments = new AccountEstimationProjectionCommitments(employerCommitments, actualProjections);

            return new AccountEstimationProjection(new Account(accountEstimation.EmployerAccountId, balance.Amount, levyFundsIn, balance.TransferAllowance, balance.RemainingTransferBalance), accountEstimationProjectionCommitments, _dateTimeService, showExpiredFunds);
        }
    }
}