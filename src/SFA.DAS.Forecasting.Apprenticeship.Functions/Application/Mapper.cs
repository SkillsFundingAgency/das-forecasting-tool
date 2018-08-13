using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Models.Payments;
using System;
using System.Threading.Tasks;

using ApiApprenticeship = SFA.DAS.Commitments.Api.Types.Apprenticeship.Apprenticeship;

namespace SFA.DAS.Forecasting.Apprenticeship.Functions.Application
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
            int NumberOfInstallments(DateTime start, DateTime end)
            {
                var count = 0;
                while (start < end) { count++; start = start.AddMonths(1); }
                return count;
            }
            var duration = NumberOfInstallments(apprenticeship.StartDate.Value, apprenticeship.EndDate.Value);
            Models.Estimation.ApprenticeshipCourse training = null;
            training = await _apprenticeshipCourseDataService?.GetApprenticeshipCourse(apprenticeship.TrainingCode);

            return new ApprenticeshipMessage
            {
                EmployerAccountId = apprenticeship.EmployerAccountId,
                SendingEmployerAccountId = apprenticeship.TransferSenderId,
                LearnerId = long.TryParse(apprenticeship.ULN, out long result) ? result : 0,
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
