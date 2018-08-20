using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Forecasting.Models.Estimation;
using System;
using System.Linq;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services
{
    public interface IApprenticehipsCourseMapper
    {
        ApprenticeshipCourse Map(StandardSummary course);
        ApprenticeshipCourse Map(FrameworkSummary course);
    }

    public class ApprenticehipsCourseMapper : IApprenticehipsCourseMapper
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

        public virtual ApprenticeshipCourse Map(FrameworkSummary course)
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
                CourseType = ApprenticeshipCourseType.Framework
            };
        }
    }
}