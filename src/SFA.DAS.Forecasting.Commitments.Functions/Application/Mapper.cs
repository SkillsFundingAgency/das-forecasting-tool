using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Models.Approvals;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Commitments.Functions.Application
{
    internal class Mapper
    {
        public Mapper()
        {
        }

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
    }
}
