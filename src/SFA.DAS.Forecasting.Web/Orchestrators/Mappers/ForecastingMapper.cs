using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
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
        private readonly IApplicationConfiguration _config;

        public ForecastingMapper(IApplicationConfiguration config)
        {
            _config = config;
        }

        public List<ProjectiontemViewModel> MapProjections(IEnumerable<AccountProjectionModel> data)
        {
            return data.Select(x =>
                new ProjectiontemViewModel
                {
                    Date = (new DateTime(x.Year, x.Month, 1)),
                    FundsIn = x.LevyFundsIn + x.TransferInCostOfTraining + x.TransferInCompletionPayments,
                    CostOfTraining = x.LevyFundedCostOfTraining + x.TransferOutCostOfTraining,
                    CompletionPayments = x.LevyFundedCompletionPayments + x.TransferOutCompletionPayments,
                    ExpiredFunds = _config.FeatureExpiredFunds ? x.ExpiredFunds : 0m,
                    Balance = _config.FeatureExpiredFunds ? x.FutureFunds : x.FutureFundsNoExpiry,
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
                LevyCredit = projectionItem.FundsIn,
                CostOfTraining = projectionItem.CostOfTraining,
                CompletionPayments = projectionItem.CompletionPayments,
                CoInvestmentEmployer = projectionItem.CoInvestmentEmployer,
                CoInvestmentGovernment = projectionItem.CoInvestmentGovernment,
                ExpiredFunds = projectionItem.ExpiredFunds,
                Balance = projectionItem.Balance
            };
        }

        public ApprenticeshipCsvItemViewModel ToCsvApprenticeship(CommitmentModel commitment, long accountId)
        {
            var model = new ApprenticeshipCsvItemViewModel
            {
                DasStartDate = commitment.StartDate.ToString("MMM-yy"),
                DasPlannedEndDate = commitment.PlannedEndDate.ToString("MMM-yy"),

                Apprenticeship = commitment.CourseName,
                ApprenticeshipLevel = commitment.CourseLevel,
                TransferToEmployer = commitment.FundingSource == FundingSource.Transfer ||
                                     commitment.FundingSource == FundingSource.ApprovedPledgeApplication ||
                                     commitment.FundingSource == FundingSource.AcceptedPledgeApplication
                    ? "Y" : "N",
                Uln = IsTransferCommitment(commitment, accountId) ? "" : commitment.LearnerId.ToString(),
                ApprenticeName = IsTransferCommitment(commitment, accountId) ? "" : commitment.ApprenticeName,
                UkPrn = commitment.ProviderId == 0 ? string.Empty : commitment.ProviderId.ToString(),
                ProviderName = commitment.ProviderName,
                TotalCost = Convert.ToInt32((commitment.MonthlyInstallment * commitment.NumberOfInstallments) + commitment.CompletionAmount),
                MonthlyTrainingCost = Convert.ToInt32(commitment.MonthlyInstallment),
                CompletionAmount = Convert.ToInt32(commitment.CompletionAmount)
            };

            if (commitment.HasHadPayment){
                model.StartDate = commitment.StartDate.ToString("MMM-yy");
                model.PlannedEndDate = commitment.PlannedEndDate.ToString("MMM-yy");
                model.Status = "Live and paid";
            }
            else if (commitment.FundingSource == FundingSource.AcceptedPledgeApplication || commitment.FundingSource == FundingSource.ApprovedPledgeApplication)
            {
                model.DasStartDate = commitment.StartDate.ToString("MMM-yy");
                model.DasPlannedEndDate = commitment.PlannedEndDate.ToString("MMM-yy");
                model.Status = commitment.FundingSource == FundingSource.ApprovedPledgeApplication
                    ? "Approved Pledge Application"
                    : "Accepted Pledge Application";
            }
            else
            {
                model.DasStartDate = commitment.StartDate.ToString("MMM-yy");
                model.DasPlannedEndDate = commitment.PlannedEndDate.ToString("MMM-yy");
                model.Status =
                    commitment.StartDate > DateTime.Today
                    ? "Waiting to start"
                    : "Live and not paid";
            }

            return model;
        }

        private static bool IsTransferCommitment(CommitmentModel commitmentModel, long accountId)
        {
            return (commitmentModel.FundingSource.Equals(FundingSource.Transfer)
                   || commitmentModel.FundingSource.Equals(FundingSource.AcceptedPledgeApplication)
                   || commitmentModel.FundingSource.Equals(FundingSource.ApprovedPledgeApplication))
                   && commitmentModel.SendingEmployerAccountId == accountId;
        }
    }
}