using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Forecasting.Models.Estimation;
using System;
using System.Linq;

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
                FundingCap = course.CurrentFundingCap,
				FundingPeriods = course.FundingPeriods.Select(m => 
                    new Models.Estimation.FundingPeriod
                    {
                        EffectiveFrom = m.EffectiveFrom ?? DateTime.MinValue,
                        EffectiveTo = m.EffectiveTo,
                        FundingCap = m.FundingCap
                    })
                    .ToList(),
				CourseType = ApprenticeshipCourseType.Standard
            };
        }
    }
}