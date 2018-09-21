using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Models.Payments;
using ApiApprenticeship = SFA.DAS.Commitments.Api.Types.Apprenticeship.Apprenticeship;

namespace SFA.DAS.Forecasting.Commitments.Functions.Application
{
    internal class Mapper
    {
        private readonly IApprenticeshipCourseDataService _apprenticeshipCourseDataService;

        public Mapper(IApprenticeshipCourseDataService apprenticeshipCourseDataService)
        {
            _apprenticeshipCourseDataService = apprenticeshipCourseDataService;
        }

        internal async Task<ApprenticeshipMessage> Map(ApiApprenticeship apprenticeship)
        {
            if (apprenticeship == null)
            {
                return new ApprenticeshipMessage();
            }

            var duration = 0;

            if (apprenticeship.EndDate.HasValue && apprenticeship.StartDate.HasValue)
            {
                duration = (apprenticeship.EndDate.Value.Year - apprenticeship.StartDate.Value.Year) * 12 +
                           apprenticeship.EndDate.Value.Month - apprenticeship.EndDate.Value.Month;
            }

            var training = await _apprenticeshipCourseDataService.GetApprenticeshipCourse(apprenticeship.TrainingCode);

            return new ApprenticeshipMessage
            {
                EmployerAccountId = apprenticeship.EmployerAccountId,
                SendingEmployerAccountId = apprenticeship.TransferSenderId,
                LearnerId = long.TryParse(apprenticeship.ULN, out var result) ? result : 0,
                ProviderId = apprenticeship.ProviderId,
                ProviderName = apprenticeship.ProviderName,
                ApprenticeshipId = apprenticeship.Id,
                ApprenticeName = $"{apprenticeship.FirstName} {apprenticeship.LastName}",
                CourseName = apprenticeship.TrainingName,
                CourseLevel = training?.Level ?? 0,
                StartDate = apprenticeship.StartDate.Value,
                PlannedEndDate = apprenticeship.EndDate.Value,
                ActualEndDate = null,
                CompletionAmount = apprenticeship.Cost.Value * 0.2M,
                MonthlyInstallment = (apprenticeship.Cost.Value * 0.8M) / duration,
                NumberOfInstallments = duration,
                FundingSource = apprenticeship.TransferSenderId == null ? FundingSource.Levy : FundingSource.Transfer
            };
        }
    }
}
