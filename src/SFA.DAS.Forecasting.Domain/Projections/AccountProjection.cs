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
                    projectionGenerationType,
                    ignoreCostOfTraining);

                _projections.Add(projection);
                lastBalance = projection.FutureFunds;
            }
        }

        private AccountProjectionModel CreateProjection(DateTime period, decimal levyFundsIn, decimal lastBalance, ProjectionGenerationType projectionGenerationType, bool ignoreCostOfTraining)
        {
	        var totalCostOfTraning = _employerCommitments.GetTotalCostOfTraining(period);
	        var completionPayments = _employerCommitments.GetTotalCompletionPayments(period);

	        var isSendingEmployer = totalCostOfTraning.TransferIn == 0
	                                && totalCostOfTraning.TransferOut != 0
	                                && completionPayments.TransferInCompletionPayment == 0
	                                && completionPayments.TransferOutCompletionPayment != 0;

			var currentBalance = GetCurrentBalance(lastBalance,
			    completionPayments.TransferOutCompletionPayment, completionPayments.TransferInCompletionPayment,
			    totalCostOfTraning.TransferOut, totalCostOfTraning.TransferIn, isSendingEmployer);

			var costOfTraining = totalCostOfTraning.LevyFunded;
	        var complPayment = completionPayments.LevyFundedCompletionPayment;

	        var transferPayments = !isSendingEmployer
		        ? totalCostOfTraning.TransferOut + completionPayments.TransferOutCompletionPayment
		        : 0;

            var moneyOut = ignoreCostOfTraining ? 0 : costOfTraining + complPayment + transferPayments;

	        var coInvestmentAmount = GetCoInvestmentAmountBasedOnCurrentBalanceAndTrainingCosts(currentBalance, moneyOut);

	        var moneyIn = levyFundsIn + totalCostOfTraning.TransferIn + completionPayments.TransferInCompletionPayment;

			var futureFunds = GetMonthEndBalance(currentBalance, moneyOut, moneyIn);

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

                CoInvestmentEmployer = coInvestmentAmount > 0 ? (coInvestmentAmount * 0.1m) * -1m : 0m,
                CoInvestmentGovernment = coInvestmentAmount > 0 ? (coInvestmentAmount * 0.9m) * -1m : 0m,
                FutureFunds = futureFunds,
                ProjectionCreationDate = DateTime.UtcNow,
                ProjectionGenerationType = projectionGenerationType
            };
            return projection;
        }

	    public decimal GetCurrentBalance(decimal lastBalance, decimal completionPaymentsTransferOut, decimal completionPaymentsTransferIn, decimal trainingCostTransferOut, decimal trainingCostTransferIn, bool isSendingEmployer)
	    {
		    if (!isSendingEmployer)
		    {
			    return lastBalance;
		    }

		    var transferCosts = completionPaymentsTransferOut + trainingCostTransferOut;
		    var currentBalance = lastBalance;

		    if (lastBalance > 0)
		    {
			    currentBalance = lastBalance - transferCosts > 0
				    ? lastBalance - transferCosts : 0;
		    }

		    return currentBalance;
		}

	    public decimal GetCoInvestmentAmountBasedOnCurrentBalanceAndTrainingCosts(decimal currentBalance, decimal trainingCosts)
	    {
		    if (currentBalance > 0 && currentBalance >= trainingCosts)
		    {
			    return 0;
		    }

		    if (currentBalance > 0)
		    {
			    return trainingCosts - currentBalance;
		    }

		    return trainingCosts;
		}

	    public decimal GetMonthEndBalance(decimal currentBalance, decimal moneyOut, decimal levyFundsIn)
	    {
			if (currentBalance > 0 && currentBalance >= moneyOut)
		    {
			    return currentBalance + levyFundsIn - moneyOut;
		    }

		    if (currentBalance > 0)
		    {
			    return levyFundsIn;
		    }

		    return currentBalance + levyFundsIn;
		}
	}
}