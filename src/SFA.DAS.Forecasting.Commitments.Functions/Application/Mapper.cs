using System.Runtime.CompilerServices;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.Forecasting.Models.Approvals;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.Forecasting.Models.Pledges;

[assembly:InternalsVisibleTo("SFA.DAS.Forecasting.Functions.UnitTests")]
namespace SFA.DAS.Forecasting.Commitments.Functions.Application
{
    
    internal class Mapper
    {
        internal ApprenticeshipMessage Map(Apprenticeship apprenticeship, long employerAccountId)
        {
            if (apprenticeship == null)
            {
                return new ApprenticeshipMessage();
            }

            var duration = (apprenticeship.EndDate.Year - apprenticeship.StartDate.Year) * 12 +
                           apprenticeship.EndDate.Month - apprenticeship.StartDate.Month;

            return new ApprenticeshipMessage
            {
                EmployerAccountId = employerAccountId,
                SendingEmployerAccountId = apprenticeship.TransferSenderId,
                LearnerId = long.TryParse(apprenticeship.Uln, out var result) ? result : 0,
                ProviderId = apprenticeship.ProviderId,
                ProviderName = apprenticeship.ProviderName,
                ApprenticeshipId = apprenticeship.Id,
                ApprenticeName = $"{apprenticeship.FirstName} {apprenticeship.LastName}",
                CourseName = apprenticeship.CourseName,
                CourseLevel = apprenticeship.CourseLevel,
                StartDate = apprenticeship.StartDate,
                PlannedEndDate = apprenticeship.EndDate,
                ActualEndDate = null,
                CompletionAmount = apprenticeship.Cost.Value * 0.2M,
                MonthlyInstallment = (apprenticeship.Cost.Value * 0.8M) / duration,
                NumberOfInstallments = duration,
                FundingSource = apprenticeship.TransferSenderId == null ? FundingSource.Levy : FundingSource.Transfer,
                PledgeApplicationId = apprenticeship.PledgeApplicationId
            };
        }

        internal ApprenticeshipMessage Map(Models.Pledges.Application x, long employerAccountId)
        {
            return new ApprenticeshipMessage
            {
                EmployerAccountId = x.EmployerAccountId,
                SendingEmployerAccountId = employerAccountId,
                LearnerId = 0,
                ProviderId = 0,
                ProviderName = string.Empty,
                ApprenticeshipId = 0,
                ApprenticeName = string.Empty,
                CourseName = x.StandardTitle,
                CourseLevel = x.StandardLevel,
                StartDate = x.StartDate,
                PlannedEndDate = x.StartDate.AddMonths(x.StandardDuration),
                ActualEndDate = null,
                CompletionAmount = x.StandardMaxFunding * 0.2M,
                MonthlyInstallment = (x.StandardMaxFunding * 0.8M) / x.StandardDuration,
                NumberOfInstallments = x.StandardDuration,
                FundingSource = x.Status == ApplicationStatus.Approved
                    ? FundingSource.ApprovedPledgeApplication
                    : FundingSource.AcceptedPledgeApplication,
                ProjectionSource = ProjectionSource.Commitment,
                PledgeApplicationId = x.Id
            };
        }
    }
}
