namespace SFA.DAS.Forecasting.Models.Estimation
{
    public class ApprenticeshipToAdd
    {
            public int? ApprenticesCount { get; set; }
            public int? NumberOfMonths { get; set; }
            public int? StartMonth { get; set; }
            public int? StartYear { get; set; }
            public decimal? TotalCost { get; set; }

            public string CourseId { get; set; }
            public ApprenticeshipCourse AppenticeshipCourse { get;set; }
        
        public decimal? CalculatedTotalCap => AppenticeshipCourse?.FundingCap != null && ApprenticesCount.HasValue
                ? AppenticeshipCourse.FundingCap * ApprenticesCount
                : null;
    }
}
