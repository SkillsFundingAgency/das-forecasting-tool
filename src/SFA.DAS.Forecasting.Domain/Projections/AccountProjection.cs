using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SFA.DAS.EmployerFinance.Types.Models;
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

        public AccountProjection(Account account, EmployerCommitments employerCommitments, IList<AccountProjectionModel> accountProjectionModels)
        {
            _account = account ?? throw new ArgumentNullException(nameof(account));
            _employerCommitments = employerCommitments ?? throw new ArgumentNullException(nameof(employerCommitments));
            _projections = accountProjectionModels != null ? accountProjectionModels.ToList() :  throw new ArgumentNullException(nameof(accountProjectionModels));
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
                                           
                if (IsProjectionTypeLevyBeingRanBeforeLevyRun(periodStart, projectionGenerationType))
                {
                    ignoreCostOfTraining = false;
                    levyFundsIn = _account.LevyDeclared;
                }

                var projection = CreateProjection(
                    periodStart.AddMonths(month),
                    levyFundsIn,
                    lastBalance, 
                    projectionGenerationType,
                    ignoreCostOfTraining,
                    periodStart.Day);

                _projections.Add(projection);
                lastBalance = projection.FutureFunds;
            }
        }

        private AccountProjectionModel CreateProjection(DateTime period, decimal levyFundsIn, decimal lastBalance, ProjectionGenerationType projectionGenerationType, bool isFirstMonth, int periodStartDay)
        {
	        var totalCostOfTraning = _employerCommitments.GetTotalCostOfTraining(period);
	        var completionPayments = _employerCommitments.GetTotalCompletionPayments(period);
            var totalCostOfPledges = _employerCommitments.GetTotalCostOfPledges(period);

			var currentBalance = GetCurrentBalance(lastBalance,completionPayments.TransferOutCompletionPayment, totalCostOfTraning.TransferOut, isFirstMonth);

            var trainingCosts = totalCostOfTraning.LevyFunded + completionPayments.LevyFundedCompletionPayment;
            
	        var coInvestmentAmount = GetCoInvestmentAmountBasedOnCurrentBalanceAndTrainingCosts(currentBalance, trainingCosts);

            var moneyOut = isFirstMonth ? coInvestmentAmount : trainingCosts - coInvestmentAmount;

            var moneyIn = isFirstMonth && projectionGenerationType == ProjectionGenerationType.LevyDeclaration ? 0: 
                levyFundsIn;

			var futureFunds = GetMonthEndBalance(currentBalance, moneyOut, moneyIn, projectionGenerationType, isFirstMonth, periodStartDay);


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

                ApprovedPledgeApplicationCost = totalCostOfPledges.ApprovedPledgeApplicationCost,
                AcceptedPledgeApplicationCost = totalCostOfPledges.AcceptedPledgeApplicationCost,
                PledgeOriginatedCommitmentCost = totalCostOfPledges.PledgeOriginatedCommitmentCost,

                CoInvestmentEmployer = coInvestmentAmount > 0 ? (coInvestmentAmount * 0.1m) : 0m,
                CoInvestmentGovernment = coInvestmentAmount > 0 ? (coInvestmentAmount * 0.9m) : 0m,
                FutureFunds = futureFunds,
                ProjectionCreationDate = DateTime.UtcNow,
                ProjectionGenerationType = projectionGenerationType
            };
            return projection;
        }

        private static bool IsProjectionTypeLevyBeingRanBeforeLevyRun(DateTime periodStart, ProjectionGenerationType projectionGenerationType)
        {
            return projectionGenerationType == ProjectionGenerationType.LevyDeclaration && periodStart.Day < 19;
        }

        public decimal GetCurrentBalance(decimal lastBalance, decimal completionPaymentsTransferOut, decimal trainingCostTransferOut, bool isFirstMonth)
	    {
		    if (!_employerCommitments.IsSendingEmployer() || isFirstMonth)
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

	    public decimal GetMonthEndBalance(decimal currentBalance, decimal moneyOut, decimal levyFundsIn, ProjectionGenerationType projectionGenerationType, bool isFirstMonth, int periodStartDay)
	    {
	        if (projectionGenerationType == ProjectionGenerationType.LevyDeclaration && isFirstMonth)
	        {
	            if (periodStartDay > 19)
	            {
	                return currentBalance;
                }
	        }

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


        public void UpdateProjectionsWithExpiredFunds(Dictionary<CalendarPeriod,decimal> expiringFunds)
        {
            foreach (var projection in _projections)
            {
                var expiredFund = expiringFunds.FirstOrDefault(w => w.Key.Year == projection.Year && w.Key.Month == projection.Month).Value;
                var expiredFundsTotal = _projections.Sum(c => c.ExpiredFunds);

                projection.ExpiredFunds = expiredFund;
                projection.FutureFundsNoExpiry = projection.FutureFunds;
                projection.FutureFunds = projection.CalculateFutureFunds(expiredFundsTotal);
            }
            
        }
	}
}