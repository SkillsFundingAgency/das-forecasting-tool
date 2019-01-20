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
        decimal MonthlyInstallmentAmount { get; }
        decimal TransferAllowance { get; set; }
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
        public decimal MonthlyInstallmentAmount { get; internal set; }
        public decimal TransferAllowance { get; set; }
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
            var accountRemainingTransferBalance = _account.RemainingTransferBalance;
            var startDate = _dateTimeService.GetCurrentDateTime().GetStartOfMonth();
            var endDate = _virtualEmployerCommitments.GetLastCommitmentPlannedEndDate().AddMonths(2).GetStartOfMonth();

            if (!_virtualEmployerCommitments.Any())
                return;

            if (endDate < startDate)
                throw new InvalidOperationException($"The start date for the earliest commitment is after the last planned end date. Account: {_account.EmployerAccountId}, Start date: {startDate}, End date: {endDate}");

            var projectionDate = startDate;
            var lastProjectedBalance = _actualAccountProjections.FirstOrDefault(c => c.Month == startDate.Month && c.Year == startDate.Year)?.FutureFunds ?? 0;
            while (projectionDate <= endDate)
            {
                if (projectionDate.Month == 5)
                    accountRemainingTransferBalance = _account.TransferAllowance;
                var projection = CreateProjection(projectionDate, accountRemainingTransferBalance, lastProjectedBalance);
                _estimatedProjections.Add(projection);
                accountRemainingTransferBalance = projection.AvailableTransferFundsBalance;
                lastProjectedBalance = projection.EstimatedProjectionBalance + _account.LevyDeclared;
                projectionDate = projectionDate.AddMonths(1);
            }

            TransferAllowance = _account.TransferAllowance;
            MonthlyInstallmentAmount = _account.LevyDeclared;
        }

        private AccountEstimationProjectionModel CreateProjection(DateTime period, decimal accountRemainingTransferBalance, decimal lastProjectedBalance)
        {
            var transferModelledCostOfTraining = _virtualEmployerCommitments.GetTotalCostOfTraining(period, true);
            var transferModelledCompletionPayments = _virtualEmployerCommitments.GetTotalCompletionPayments(period, true);
            var allModelledCostOfTraining = _virtualEmployerCommitments.GetTotalCostOfTraining(period);
            var allModelledCompletionPayments = _virtualEmployerCommitments.GetTotalCompletionPayments(period);
            var actualAccountProjection = _actualAccountProjections.FirstOrDefault(c=>c.Month == period.Month && c.Year == period.Year);

            
            var projection = new AccountEstimationProjectionModel
            {
                Month = (short)period.Month,
                Year = (short)period.Year,
                ProjectionGenerationType = actualAccountProjection?.ProjectionGenerationType ?? ProjectionGenerationType.LevyDeclaration,
                TransferModelledCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = transferModelledCostOfTraining.LevyFunded,
                    LevyCompletionPayments = transferModelledCompletionPayments.LevyFundedCompletionPayment,
                    TransferInCostOfTraining = transferModelledCostOfTraining.TransferIn,
                    TransferInCompletionPayments = transferModelledCompletionPayments.TransferInCompletionPayment,
                    TransferOutCostOfTraining = transferModelledCostOfTraining.TransferOut,
                    TransferOutCompletionPayments = transferModelledCompletionPayments.TransferOutCompletionPayment
                },
                AllModelledCosts = new AccountEstimationProjectionModel.Cost
                {
                    LevyCostOfTraining = allModelledCostOfTraining.LevyFunded,
                    LevyCompletionPayments = allModelledCompletionPayments.LevyFundedCompletionPayment,
                    TransferInCostOfTraining = allModelledCostOfTraining.TransferIn,
                    TransferInCompletionPayments = allModelledCompletionPayments.TransferInCompletionPayment,
                    TransferOutCostOfTraining = allModelledCostOfTraining.TransferOut,
                    TransferOutCompletionPayments = allModelledCompletionPayments.TransferOutCompletionPayment
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

            if (_estimatedProjections.Any())
            {
                lastProjectedBalance = lastProjectedBalance - projection.ActualCosts.FundsOut;
            }

            //transfer projection
            projection.AvailableTransferFundsBalance = accountRemainingTransferBalance - projection.TransferFundsOut;

            //estimate balance
            projection.EstimatedProjectionBalance = lastProjectedBalance - projection.AllModelledCosts.FundsOut; 


            return projection;
        }
    }
}