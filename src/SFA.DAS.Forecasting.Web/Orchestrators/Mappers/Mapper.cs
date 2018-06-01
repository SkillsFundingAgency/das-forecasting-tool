using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.Forecasting.Models.Projections;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Mappers
{
    public interface IForecastingMapper
    {
        List<ProjectiontemViewModel> MapProjections(IEnumerable<AccountProjectionModel> data);
        BalanceCsvItemViewModel ToCsvBalance(ProjectiontemViewModel projectionItem);
        ApprenticeshipCsvItemViewModel ToCsvApprenticeship(CommitmentModel commitment, long accountId);
    }

    public class ForecastingMapper : IForecastingMapper
    {
        public List<ProjectiontemViewModel> MapProjections(IEnumerable<AccountProjectionModel> data)
        {
            return data.Select(x =>
                new ProjectiontemViewModel
                {
                    Date = (new DateTime(x.Year, x.Month, 1)),
                    LevyCredit = x.LevyFundsIn,
                    CostOfTraining = x.LevyFundedCostOfTraining + x.TransferOutCostOfTraining,
                    CompletionPayments = x.LevyFundedCompletionPayments + x.TransferOutCompletionPayments,
                    ExpiredFunds = 0,
                    Balance = x.FutureFunds,
                    CoInvestmentEmployer = x.CoInvestmentEmployer,
                    CoInvestmentGovernment = x.CoInvestmentGovernment
                })
                .ToList();
        }

        public BalanceCsvItemViewModel ToCsvBalance(ProjectiontemViewModel projectionItem)
        {
            return new BalanceCsvItemViewModel
            {
                Date = projectionItem.Date.ToGdsFormatShortMonthWithoutDay(),
                LevyCredit = projectionItem.LevyCredit,
                CostOfTraining = projectionItem.CostOfTraining,
                CompletionPayments = projectionItem.CompletionPayments,
                CoInvestmentEmployer = projectionItem.CoInvestmentEmployer,
                CoInvestmentGovernment = projectionItem.CoInvestmentGovernment,
                Balance = projectionItem.Balance
            };
        }

        public ApprenticeshipCsvItemViewModel ToCsvApprenticeship(CommitmentModel commitment, long accountId)
        {
            return new ApprenticeshipCsvItemViewModel
            {
                StartDate = commitment.StartDate.ToString("MMM-yy"),
                PlannedEndDate = commitment.PlannedEndDate.ToString("MMM-yy"),
                Apprenticeship = commitment.CourseName,
                ApprenticeshipLevel = commitment.CourseLevel,
                TransferToEmployer = commitment.FundingSource == FundingSource.Transfer ? "Y" : "N",
                Uln = IsTransferCommitment(commitment, accountId) ? "" : commitment.LearnerId.ToString(),
                ApprenticeName = IsTransferCommitment(commitment, accountId) ? "" : commitment.ApprenticeName,
                UkPrn = commitment.ProviderId,
                ProviderName = commitment.ProviderName,
                TotalCost = Convert.ToInt32((commitment.MonthlyInstallment * commitment.NumberOfInstallments) + commitment.CompletionAmount),
                MonthlyTrainingCost = Convert.ToInt32(commitment.MonthlyInstallment),
                CompletionAmount = Convert.ToInt32(commitment.CompletionAmount)
            };
        }

        private static bool IsTransferCommitment(CommitmentModel commitmentModel, long accountId)
        {
            return commitmentModel.FundingSource.Equals(FundingSource.Transfer) && commitmentModel.SendingEmployerAccountId != accountId;
        }
    }
}