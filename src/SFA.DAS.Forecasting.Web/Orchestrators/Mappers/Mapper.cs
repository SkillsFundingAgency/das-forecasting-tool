using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.Forecasting.Models.Projections;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Mappers
{
    public class ForecastingMapper
    {
        public IEnumerable<BalanceItemViewModel> MapProjections(IEnumerable<AccountProjectionModel> data)
        {
            return data.Select(x =>
                new BalanceItemViewModel
                {
                    Date = (new DateTime(x.Year, x.Month, 1)),
                    LevyCredit = x.LevyFundsIn,
                    CostOfTraining = x.LevyFundedCostOfTraining + x.TransferOutCostOfTraining,
                    CompletionPayments = x.LevyFundedCompletionPayments + x.TransferOutCompletionPayments,
                    ExpiredFunds = 0,
                    Balance = x.FutureFunds,
                    CoInvestmentEmployer = x.CoInvestmentEmployer,
                    CoInvestmentGovernment = x.CoInvestmentGovernment
                });
        }

        public BalanceCsvItemViewModel ToCsvBalance(CommitmentModel x, long accountId)
        {
            return new BalanceCsvItemViewModel
            {
                StartDate = x.StartDate.ToString("MMM-yy"),
                PlannedEndDate = x.PlannedEndDate.ToString("MMM-yy"),
                Apprenticeship = x.CourseName,
                ApprenticeshipLevel = x.CourseLevel,
                TransferToEmployer = x.FundingSource.Equals(FundingSource.Transfer) ? "Y" : "N" ,
                Uln = IsTransferCommitment(x, accountId) ? "" : x.LearnerId.ToString(),
                ApprenticeName = IsTransferCommitment(x, accountId) ? "" : x.ApprenticeName,
                UkPrn = x.ProviderId,
                ProviderName = x.ProviderName,
                TotalCost = Convert.ToInt32((x.MonthlyInstallment * x.NumberOfInstallments) + x.CompletionAmount),
                MonthlyTrainingCost = Convert.ToInt32(x.MonthlyInstallment),
                CompletionAmount = Convert.ToInt32(x.CompletionAmount)
            };

        }

        private static bool IsTransferCommitment(CommitmentModel commitmentModel, long accountId)
        {
            return commitmentModel.FundingSource.Equals(FundingSource.Transfer) && commitmentModel.SendingEmployerAccountId != accountId;
        }
    }
}