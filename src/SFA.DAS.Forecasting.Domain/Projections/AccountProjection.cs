using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.Projections
{
    public class AccountProjection
    {
        private readonly Account _account;
        private readonly EmployerCommitments _employerCommitments;
        private readonly List<AccountProjectionReadModel> _projections;
        public ReadOnlyCollection<AccountProjectionReadModel> Projections => _projections.AsReadOnly();
        public AccountProjection(Account account, EmployerCommitments employerCommitments)
        {
            _account = account ?? throw new ArgumentNullException(nameof(account));
            _employerCommitments = employerCommitments ?? throw new ArgumentNullException(nameof(employerCommitments));
            _projections = new List<AccountProjectionReadModel>();
        }

        public void BuildLevyTriggeredProjections(DateTime periodStart, int numberOfMonths)
        {
            BuildProjections(periodStart, numberOfMonths, ProjectionGenerationType.LevyDeclaration);
        }

        public void BuildPayrollPeriodEndTriggeredProjections(DateTime periodStart, int numberOfMonths)
        {
            BuildProjections(periodStart, numberOfMonths, ProjectionGenerationType.PayrollPeriodEnd);
        }

        private void BuildProjections(DateTime periodStart, int numberOfMonths, ProjectionGenerationType projectionGenerationType)
        {
            var startMonth = 0;

            _projections.Clear();
            var lastBalance = _account.Balance;
            for (var month = startMonth; month <= numberOfMonths; month++)
            {
                var fundsIn = projectionGenerationType == ProjectionGenerationType.LevyDeclaration && month == startMonth
                        ? 0 : _account.LevyDeclared;
                var ignoreCostOfTraining = month == startMonth;

                var projection = CreateProjection(
                    periodStart.AddMonths(month),
                    fundsIn,
                    lastBalance, 
                    ProjectionGenerationType.LevyDeclaration,
                    ignoreCostOfTraining);

                _projections.Add(projection);
                lastBalance = projection.FutureFunds;
            }
        }

        private AccountProjectionReadModel CreateProjection(DateTime period, decimal fundsIn, decimal lastBalance, ProjectionGenerationType projectionGenerationType, bool ignoreCostOfTraining)
        {
            var totalCostOfTraning = _employerCommitments.GetTotalCostOfTraining(period);
            var completionPayments = _employerCommitments.GetTotalCompletionPayments(period);
            var commitments = totalCostOfTraning.Item2;
            commitments.AddRange(completionPayments.Item2);
            commitments = commitments.Distinct().ToList();

            var cost = ignoreCostOfTraining ? 0 : totalCostOfTraning.Item1 + completionPayments.Item1;
            var balance = lastBalance + fundsIn - cost;

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
                ProjectionGenerationType = projectionGenerationType,
                Commitments = commitments
            };
            return projection;
        }
    }
}