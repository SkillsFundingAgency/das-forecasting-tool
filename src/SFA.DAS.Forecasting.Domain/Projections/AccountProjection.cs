using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.Projections
{
    public class AccountProjection
    {
        public long EmployerAccountId => _account.EmployerAccountId;
        private readonly Account _account;
        private readonly EmployerCommitments _employerCommitments;
        private readonly List<AccountProjectionModel> _projections;
        public ReadOnlyCollection<AccountProjectionModel> Projections => _projections.AsReadOnly();
        public AccountProjection(Account account, EmployerCommitments employerCommitments)
        {
            _account = account ?? throw new ArgumentNullException(nameof(account));
            _employerCommitments = employerCommitments ?? throw new ArgumentNullException(nameof(employerCommitments));
            _projections = new List<AccountProjectionModel>(49);
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
                var levyFundsIn = projectionGenerationType == ProjectionGenerationType.LevyDeclaration && month == startMonth
                        ? 0 : _account.LevyDeclared;
                var ignoreCostOfTraining = month == startMonth;

                var projection = CreateProjection(
                    periodStart.AddMonths(month),
                    levyFundsIn,
                    lastBalance, 
                    ProjectionGenerationType.LevyDeclaration,
                    ignoreCostOfTraining);

                _projections.Add(projection);
                lastBalance = projection.FutureFunds;
            }
        }

        private AccountProjectionModel CreateProjection(DateTime period, decimal levyFundsIn, decimal lastBalance, ProjectionGenerationType projectionGenerationType, bool ignoreCostOfTraining)
        {
            var totalCostOfTraning = _employerCommitments.GetTotalCostOfTraining(period);
            var completionPayments = _employerCommitments.GetTotalCompletionPayments(period);
            
            var costOfTraining = totalCostOfTraning.LevyFunded + totalCostOfTraning.TransferOut;
            var complPayment = completionPayments.LevyFundedCompletionPayment + (completionPayments.TransferOutCompletionPayment - completionPayments.TransferInCompletionPayment);

            var moneyOut = ignoreCostOfTraining ? 0 : costOfTraining + complPayment;
            var moneyIn = lastBalance + levyFundsIn + totalCostOfTraning.TransferIn;

            var balance = moneyIn - moneyOut;

            var projection = new AccountProjectionModel
            {
                LevyFundsIn = _account.LevyDeclared,
                EmployerAccountId = _account.EmployerAccountId,
                Month = (short)period.Month,
                Year = (short)period.Year,

                LevyFundedCostOfTraining = totalCostOfTraning.LevyFunded,
                TransferInCostOfTraining = totalCostOfTraning.TransferIn,
                TransferOutCostOfTraining = totalCostOfTraning.TransferOut,

                LevyFundedCompletionPayments = completionPayments.LevyFundedCompletionPayment,
                TransferInCompletionPayments = completionPayments.TransferInCompletionPayment,
                TransferOutCompletionPayments = completionPayments.TransferOutCompletionPayment,

                CoInvestmentEmployer = balance < 0 ? (balance * 0.1m) * -1m : 0m,
                CoInvestmentGovernment = balance < 0 ? (balance * 0.9m) * -1m : 0m,
                FutureFunds = balance < 0 ? 0m : balance,
                ProjectionCreationDate = DateTime.UtcNow,
                ProjectionGenerationType = projectionGenerationType
            };
            return projection;
        }
    }
}