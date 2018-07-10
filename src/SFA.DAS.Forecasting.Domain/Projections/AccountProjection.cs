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
        internal AccountProjectionModel Model { get; private set; }
        public ReadOnlyCollection<AccountProjectionMonth> Projections => Model.Projections.AsReadOnly();
        public AccountProjection(Account account, EmployerCommitments employerCommitments)
        {
            _account = account ?? throw new ArgumentNullException(nameof(account));
            _employerCommitments = employerCommitments ?? throw new ArgumentNullException(nameof(employerCommitments));
            Model = new AccountProjectionModel { Id = account.EmployerAccountId.ToString(), EmployerAccountId = account.EmployerAccountId };
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
            Model.ProjectionGenerationType = projectionGenerationType;
            Model.ProjectionCreationDate = DateTime.Now;
            Model.Projections.Clear();
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
                projection.Id = month;
                Model.Projections.Add(projection);
                lastBalance = projection.FutureFunds;
            }
        }

        private AccountProjectionMonth CreateProjection(DateTime period, decimal levyFundsIn, decimal lastBalance, ProjectionGenerationType projectionGenerationType, bool ignoreCostOfTraining)
        {
            var totalCostOfTraning = _employerCommitments.GetTotalCostOfTraining(period);
            var completionPayments = _employerCommitments.GetTotalCompletionPayments(period);
            //TODO: optimise
            Model.Commitments = Model.Commitments
                .Concat(totalCostOfTraning.CommitmentIds)
                .Concat(completionPayments.CommitmentIds)
                .Distinct().ToList();

            var costOfTraining = totalCostOfTraning.LevyFunded + totalCostOfTraning.TransferOut;
            var complPayment = completionPayments.LevyFundedCompletionPayment + completionPayments.TransferOutCompletionPayment;

            var moneyOut = ignoreCostOfTraining ? 0 : costOfTraining + complPayment;
            var moneyIn = lastBalance + levyFundsIn + totalCostOfTraning.TransferIn;

            var balance = moneyIn - moneyOut;

            var projection = new AccountProjectionMonth
            {
                LevyFundsIn = _account.LevyDeclared,
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
                //Commitments = commitments.Select(commitmentId => new AccountProjectionCommitment { CommitmentId = commitmentId }).ToList()
            };

            return projection;
        }
    }
}