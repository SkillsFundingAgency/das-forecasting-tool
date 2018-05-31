using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Shared;
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
        private readonly IDateTimeService _dateTimeService;
        private readonly Account _account;
        private readonly EmployerCommitments _virtualEmployerCommitments;
        private readonly List<AccountEstimationProjectionModel> _estimatedProjections;
        private readonly IList<AccountProjectionModel> _actualAccountProjections;
        public ReadOnlyCollection<AccountEstimationProjectionModel> Projections => _estimatedProjections.AsReadOnly();
        public AccountEstimationProjection(Account account, AccountEstimationProjectionCommitments accountEstimationProjectionCommitments, IDateTimeService dateTimeService)
        {
            _dateTimeService = dateTimeService;
            _account = account ?? throw new ArgumentNullException(nameof(account));
            if (accountEstimationProjectionCommitments == null)
            {
                throw new ArgumentNullException(nameof(accountEstimationProjectionCommitments));
            }
            _virtualEmployerCommitments = accountEstimationProjectionCommitments.VirtualEmployerCommitments ?? throw new ArgumentNullException(nameof(accountEstimationProjectionCommitments.VirtualEmployerCommitments));
            _actualAccountProjections = accountEstimationProjectionCommitments.ActualAccountProjections.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(accountEstimationProjectionCommitments.ActualAccountProjections));
            _estimatedProjections = new List<AccountEstimationProjectionModel>();
        }

        public void BuildProjections()
        {
            _estimatedProjections.Clear();
            var lastBalance = _account.RemainingTransferBalance;
            var startDate = _dateTimeService.GetCurrentDateTime().GetStartOfMonth();
            var endDate = _virtualEmployerCommitments.GetLastCommitmentPlannedEndDate().AddMonths(2).GetStartOfMonth();
            if (endDate < startDate)
                throw new InvalidOperationException($"The start date for the earliest commitment is after the last planned end date. Account: {_account.EmployerAccountId}, Start date: {startDate}, End date: {endDate}");

            var projectionDate = startDate;
            while (projectionDate <= endDate)
            {
                if (projectionDate.Month == 5)
                    lastBalance = _account.TransferAllowance;
                var projection = CreateProjection(projectionDate, lastBalance);
                _estimatedProjections.Add(projection);
                lastBalance = projection.FutureFunds;
                projectionDate = projectionDate.AddMonths(1);
            }
        }

        private AccountEstimationProjectionModel CreateProjection(DateTime period, decimal lastBalance)
        {
            var modelledCostOfTraining = _virtualEmployerCommitments.GetTotalCostOfTraining(period);
            var modelledCompletionPayments = _virtualEmployerCommitments.GetTotalCompletionPayments(period);
            var actualAccountProjection = _actualAccountProjections.FirstOrDefault(c=>c.Month == period.Month && c.Year == period.Year);
			            
            var projection = new AccountEstimationProjectionModel
            {
                Month = (short)period.Month,
                Year = (short)period.Year,

                ModelledCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = modelledCostOfTraining.LevyFunded,
                    LevyCompletionPayments = modelledCompletionPayments.LevyFundedCompletionPayment,
                    TransferInCostOfTraining = modelledCostOfTraining.TransferIn,
                    TransferInCompletionPayments = modelledCompletionPayments.TransferInCompletionPayment,
                    TransferOutCostOfTraining = modelledCostOfTraining.TransferOut,
                    TransferOutCompletionPayments = modelledCompletionPayments.TransferOutCompletionPayment
                },
                ActualCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = actualAccountProjection?.LevyFundedCostOfTraining ?? 0m,
                    LevyCompletionPayments = actualAccountProjection?.LevyFundedCompletionPayments ?? 0m,
                    TransferInCostOfTraining = actualAccountProjection?.TransferInCostOfTraining ?? 0m,
                    TransferInCompletionPayments = actualAccountProjection?.TransferInCompletionPayments ?? 0m,
                    TransferOutCostOfTraining = actualAccountProjection?.TransferOutCostOfTraining ?? 0m,
                    TransferOutCompletionPayments = actualAccountProjection?.TransferOutCompletionPayments ?? 0m
                }
            };

            var balance = lastBalance + projection.ModelledCosts.TransferFundsIn + projection.ActualCosts.TransferFundsIn -
                      projection.ModelledCosts.FundsOut - projection.ActualCosts.FundsOut;
            projection.FutureFunds = balance;
            return projection;
        }
    }
}