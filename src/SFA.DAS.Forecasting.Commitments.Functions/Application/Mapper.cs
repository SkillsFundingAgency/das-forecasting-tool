using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Commitments.Functions.Application
{
    internal class Mapper
    {
        private readonly IApprenticeshipCourseDataService _apprenticeshipCourseDataService;

        public Mapper(IApprenticeshipCourseDataService apprenticeshipCourseDataService)
        {
            _apprenticeshipCourseDataService = apprenticeshipCourseDataService;
        }

        internal async Task<ApprenticeshipMessage> Map(GetApprenticeshipsResponse.ApprenticeshipDetailsResponse apprenticeship)
        {
            if (apprenticeship == null)
            {
                return new ApprenticeshipMessage();
            }

            var duration = (apprenticeship.EndDate.Year - apprenticeship.StartDate.Year) * 12 +
                           apprenticeship.EndDate.Month - apprenticeship.StartDate.Month;

            var training = await _apprenticeshipCourseDataService.GetApprenticeshipCourse(apprenticeship.CourseCode);

            return new ApprenticeshipMessage
            {
                EmployerAccountId = apprenticeship.AccountLegalEntityId,
                SendingEmployerAccountId = apprenticeship.TransferSenderId,
                LearnerId = long.TryParse(apprenticeship.Uln, out var result) ? result : 0,
                ProviderId = apprenticeship.ProviderId,
                ProviderName = apprenticeship.ProviderName,
                ApprenticeshipId = apprenticeship.Id,
                ApprenticeName = $"{apprenticeship.FirstName} {apprenticeship.LastName}",
                CourseName = apprenticeship.CourseName,
                CourseLevel = training?.Level ?? 0,
                StartDate = apprenticeship.StartDate,
                PlannedEndDate = apprenticeship.EndDate,
                ActualEndDate = null,
                CompletionAmount = apprenticeship.Cost.Value * 0.2M,
                MonthlyInstallment = (apprenticeship.Cost.Value * 0.8M) / duration,
                NumberOfInstallments = duration,
                FundingSource = apprenticeship.TransferSenderId == null ? FundingSource.Levy : FundingSource.Transfer
            };
        }
    }
}
