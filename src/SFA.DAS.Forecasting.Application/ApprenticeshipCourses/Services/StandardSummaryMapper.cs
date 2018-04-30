using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services
{
    public interface IStandardSummaryMapper
    {
        ApprenticeshipCourse Map(StandardSummary course);
    }

    public class StandardSummaryMapper: IStandardSummaryMapper
    {
        public virtual ApprenticeshipCourse Map(StandardSummary course)
        {
            return new ApprenticeshipCourse
            {
                Id = course.Id,
                Level = course.Level,
                Duration = course.Duration,
                Title = course.Title,
                FundingCap = course.MaxFunding,
                CourseType = ApprenticeshipCourseType.Standard
            };
        }
    }
}