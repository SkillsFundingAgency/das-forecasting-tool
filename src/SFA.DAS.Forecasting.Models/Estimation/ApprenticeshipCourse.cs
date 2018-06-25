using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.Forecasting.Models.Estimation
{
    public class ApprenticeshipCourse: IDocument
    {
        public string Id { get; set; }

		public string Title { get; set; }

		public decimal FundingCap { get; set; }

        public int Level { get; set; }

		public int Duration { get; set; }

		public ApprenticeshipCourseType CourseType { get; set; }

		public List<FundingPeriod> FundingPeriods { get; set; }
    }
}
