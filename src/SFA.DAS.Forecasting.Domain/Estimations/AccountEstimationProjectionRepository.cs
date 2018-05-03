using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Commitments.Validation;
using SFA.DAS.Forecasting.Domain.Events;
using SFA.DAS.Forecasting.Models.Balance;
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

        public AccountEstimationProjectionRepository(IAccountEstimationRepository accountEstimationRepository,
            ICurrentBalanceRepository currentBalanceRepository)
        {
            _accountEstimationRepository = accountEstimationRepository ?? throw new ArgumentNullException(nameof(accountEstimationRepository));
            _currentBalanceRepository = currentBalanceRepository ?? throw new ArgumentNullException(nameof(currentBalanceRepository));
        }

        public async Task<IAccountEstimationProjection> Get(long accountId)
        {
            var accountEstimation = await _accountEstimationRepository.Get(accountId);
            return await Get(accountEstimation);
        }

        public async Task<IAccountEstimationProjection> Get(AccountEstimation accountEstimation)
        {
            var balance = await _currentBalanceRepository.Get(accountEstimation.EmployerAccountId);
            var commitments = new List<CommitmentModel>();
            foreach (var virtualApprenticeships in accountEstimation.VirtualApprenticeships)
            {
                for(var i = 0; i < virtualApprenticeships.ApprenticesCount; i++) 
                    commitments.Add(new CommitmentModel
                    {
                        CompletionAmount = virtualApprenticeships.TotalCompletionAmount / virtualApprenticeships.ApprenticesCount,
                        EmployerAccountId = accountEstimation.EmployerAccountId,
                        ActualEndDate = null,
                        MonthlyInstallment = virtualApprenticeships.TotalInstallmentAmount / virtualApprenticeships.ApprenticesCount,
                        NumberOfInstallments = virtualApprenticeships.TotalInstallments,
                        PlannedEndDate = virtualApprenticeships.StartDate.AddMonths(virtualApprenticeships.TotalInstallments),
                        StartDate = virtualApprenticeships.StartDate,
                        
                                           
                    });
            }
            var employerCommitments = new EmployerCommitments(accountEstimation.EmployerAccountId, commitments, null);
            return new AccountEstimationProjection(new Account(accountEstimation.EmployerAccountId, balance.Amount, 0, balance.TransferAllowance, balance.RemainingTransferBalance), employerCommitments);
        }
    }
}