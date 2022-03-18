using System.Collections.Generic;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi
{
    public class GetStandardsApiRequest :IGetApiRequest
    {
        public string GetUrl => $"courses/standards";
    }
    
    public class ApprenticeshipCourseStandardsResponse
    {
        public List<ApprenticeshipCourse> Standards { get; set; }
    }
}