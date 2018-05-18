using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.Estimations
{
    public interface IAccountEstimationProjection
    {
        ReadOnlyCollection<AccountEstimationProjectionModel> Projections { get; }

        void BuildProjections();
    }

    public class AccountEstimationProjection : IAccountEstimationProjection
    {
        private readonly Account _account;
        private readonly EmployerCommitments _virtualEmployerCommitments;
        private readonly IList<AccountEstimationProjectionModel> _projections;
        private readonly IList<AccountProjectionModel> _actualAccountProjections;
        public ReadOnlyCollection<AccountEstimationProjectionModel> Projections => _projections.ToList().AsReadOnly();
        public AccountEstimationProjection(Account account, AccountEstimationProjectionCommitments accountEstimationProjectionCommitments)
        {
            _account = account ?? throw new ArgumentNullException(nameof(account));
            if (accountEstimationProjectionCommitments == null)
            {
                throw new ArgumentNullException(nameof(accountEstimationProjectionCommitments));
            }
            _virtualEmployerCommitments = accountEstimationProjectionCommitments.VirtualEmployerCommitments ?? throw new ArgumentNullException(nameof(accountEstimationProjectionCommitments.VirtualEmployerCommitments));
            _actualAccountProjections = accountEstimationProjectionCommitments.ActualAccountProjections ?? throw new ArgumentNullException(nameof(accountEstimationProjectionCommitments.ActualAccountProjections));
            _projections = new List<AccountEstimationProjectionModel>();
        }

        public void BuildProjections()
        {
            _projections.Clear();
            var lastBalance = _account.RemainingTransferBalance;
            var startDate = _virtualEmployerCommitments.GetEarliestCommitmentStartDate().GetStartOfMonth();
            var endDate = _virtualEmployerCommitments.GetLastCommitmentPlannedEndDate().AddMonths(2).GetStartOfMonth();
            if (endDate < startDate)
                throw new InvalidOperationException($"The start date for the earliest commitment is after the last planned end date. Account: {_account.EmployerAccountId}, Start date: {startDate}, End date: {endDate}");

            var projectionDate = startDate.AddMonths(1).GetStartOfMonth();
            while (projectionDate <= endDate)
            {
                if (projectionDate.Month == 5)
                    lastBalance = _account.TransferAllowance;
                var projection = CreateProjection(projectionDate, lastBalance);
                _projections.Add(projection);
                lastBalance = CalculateLastBalance(projection);
                projectionDate = projectionDate.AddMonths(1);
            }
        }

        private static decimal CalculateLastBalance(AccountEstimationProjectionModel projection)
        {
            return projection.FutureFunds - 
                projection.LevyFundedCostOfTraining  - 
                projection.LevyFundedCompletionPayment - 
                projection.TransferOutTotalCostOfTraining -
                projection.TransferOutCompletionPayments -
                projection.ActualCommittedTransferCost -
                projection.ActualCommittedTransferCompletionCost;
        }

        private AccountEstimationProjectionModel CreateProjection(DateTime period, decimal lastBalance)
        {
            var totalCostOfTraning = _virtualEmployerCommitments.GetTotalCostOfTraining(period);
            var completionPayments = _virtualEmployerCommitments.GetTotalCompletionPayments(period);
            var actualAccountProjection = _actualAccountProjections.FirstOrDefault(c=>c.Month == period.Month && c.Year == period.Year);
			
			            
            var balance = lastBalance;
            var projection = new AccountEstimationProjectionModel
            {
                Month = (short)period.Month,
                Year = (short)period.Year,
                LevyFundedCostOfTraining = totalCostOfTraning.LevyFunded,

                TransferInTotalCostOfTraining = totalCostOfTraning.TransferIn,
                TransferOutTotalCostOfTraining = totalCostOfTraning.TransferOut,
                LevyFundedCompletionPayment = completionPayments.LevyFundedCompletionPayment,
                TransferInCompletionPayments = completionPayments.TransferInCompletionPayment,
                TransferOutCompletionPayments = completionPayments.TransferOutCompletionPayment,

                ActualCommittedTransferCost = actualAccountProjection?.TransferOutCostOfTraining ?? 0m,
                ActualCommittedTransferCompletionCost = actualAccountProjection?.TransferOutCompletionPayments ?? 0m,

                FutureFunds = balance < 0 ? 0m : balance,
                
            };
            return projection;
        }

    }
}