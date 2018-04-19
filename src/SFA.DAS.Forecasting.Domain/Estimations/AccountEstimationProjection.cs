using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Estimations.Services;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.Estimations
{
    public interface IAccountEstimationProjection
    {
        ReadOnlyCollection<AccountProjectionReadModel> Projections { get; }

        void BuildProjections();
    }

    public class AccountEstimationProjection : IAccountEstimationProjection
    {
        private readonly Account _account;
        private readonly EmployerCommitments _virtualEmployerCommitments;
        private readonly List<AccountProjectionReadModel> _projections;
        public ReadOnlyCollection<AccountProjectionReadModel> Projections => _projections.AsReadOnly();
        public AccountEstimationProjection(Account account, EmployerCommitments virtualEmployerCommitments)
        {
            _account = account ?? throw new ArgumentNullException(nameof(account));
            _virtualEmployerCommitments = virtualEmployerCommitments ?? throw new ArgumentNullException(nameof(virtualEmployerCommitments));
            _projections = new List<AccountProjectionReadModel>();
        }

        public void BuildProjections()
        {
            _projections.Clear();
            var lastBalance = _account.RemainingTransferBalance;
            var startDate = _virtualEmployerCommitments.GetEarliestCommitmentStartDate().GetStartOfMonth();
            var endDate = _virtualEmployerCommitments.GetLastCommitmentPlannedEndDate().AddMonths(1).GetStartOfMonth();
            if (endDate < startDate)
                throw new InvalidOperationException($"The start date for the earliest commitment is after the last planned end date. Account: {_account.EmployerAccountId}, Start date: {startDate}, End date: {endDate}");

            var projectionDate = startDate.AddMonths(1).GetStartOfMonth();
            while (projectionDate <= endDate)
            {
                if (projectionDate.Month == 4)
                    lastBalance = _account.TransferAllowance;
                var projection = CreateProjection(projectionDate, lastBalance);
                _projections.Add(projection);
                lastBalance = projection.FutureFunds - projection.TotalCostOfTraining - projection.CompletionPayments;
                projectionDate = projectionDate.AddMonths(1);
            }
        }

        private AccountProjectionReadModel CreateProjection(DateTime period, decimal lastBalance)
        {
            var totalCostOfTraning = _virtualEmployerCommitments.GetTotalCostOfTraining(period);
            var completionPayments = _virtualEmployerCommitments.GetTotalCompletionPayments(period);
            var commitments = totalCostOfTraning.Item2;
            commitments.AddRange(completionPayments.Item2);
            commitments = commitments.Distinct().ToList();
            var balance = lastBalance;
            var projection = new AccountProjectionReadModel
            {
                FundsIn = _account.LevyDeclared,
                EmployerAccountId = _account.EmployerAccountId,
                Month = (short)period.Month,
                Year = (short)period.Year,
                TotalCostOfTraining = totalCostOfTraning.Item1,
                CompletionPayments = completionPayments.Item1,
                CoInvestmentEmployer = balance < 0 ? (balance * 0.1m) * -1m : 0m,
                CoInvestmentGovernment = balance < 0 ? (balance * 0.9m) * -1m : 0m,
                FutureFunds = balance < 0 ? 0m : balance,
                ProjectionCreationDate = DateTime.UtcNow,
                ProjectionGenerationType = ProjectionGenerationType.PayrollPeriodEnd,
                Commitments = commitments
            };
            return projection;
        }

    }
}