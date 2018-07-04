using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class CourseViewModel
    {
        public string CourseId { get; set; }
        public int NumberOfMonths { get; set; }
        public IList<FundingPeriodViewModel> FundingPeriods { get; internal set; }
    }
}