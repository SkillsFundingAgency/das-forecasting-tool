using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.Estimations
{
    public interface IAccountEstimationProjectionRepository
    {
        Task<IAccountEstimationProjection> Get(AccountEstimation accountEstimation);
    }

    public class AccountEstimationProjectionRepository: IAccountEstimationProjectionRepository
    {
        private readonly ICurrentBalanceRepository _currentBalanceRepository;

        public AccountEstimationProjectionRepository(ICurrentBalanceRepository currentBalanceRepository)
        {
            _currentBalanceRepository = currentBalanceRepository ?? throw new ArgumentNullException(nameof(currentBalanceRepository));
        }

        public async Task<IAccountEstimationProjection> Get(AccountEstimation accountEstimation)
        {
            var balance = await _currentBalanceRepository.Get(accountEstimation.EmployerAccountId);
            var commitments = new List<CommitmentModel>();
            foreach (var virtualApprenticeships in accountEstimation.VirtualApprenticeships)
            {
                for (var i = 0; i < virtualApprenticeships.ApprenticesCount; i++)
                {
                    commitments.Add(new CommitmentModel
                    {
                        CompletionAmount = virtualApprenticeships.TotalCompletionAmount / virtualApprenticeships.ApprenticesCount,
                        SendingEmployerAccountId = accountEstimation.EmployerAccountId,
                        ActualEndDate = null,
                        MonthlyInstallment = virtualApprenticeships.TotalInstallmentAmount / virtualApprenticeships.ApprenticesCount,
                        NumberOfInstallments = virtualApprenticeships.TotalInstallments,
                        PlannedEndDate = virtualApprenticeships.StartDate.AddMonths(virtualApprenticeships.TotalInstallments),
                        StartDate = virtualApprenticeships.StartDate,
                        FundingSource = virtualApprenticeships.FundingSource != 0 ? virtualApprenticeships.FundingSource : FundingSource.Transfer
                    });
                }
            }
            var employerCommitments = new EmployerCommitments(accountEstimation.EmployerAccountId, commitments);
            return new AccountEstimationProjection(new Account(accountEstimation.EmployerAccountId, balance.Amount, 0, balance.TransferAllowance, balance.RemainingTransferBalance), employerCommitments);
        }
    }
}