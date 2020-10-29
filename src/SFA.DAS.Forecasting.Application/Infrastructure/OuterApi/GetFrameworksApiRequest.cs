using System.Collections.Generic;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi
{
    public class GetFrameworksApiRequest :IGetApiRequest
    {
        public string GetUrl => $"frameworks";
    }

    public class ApprenticeshipCourseFrameworkResponse
    {
        public List<ApprenticeshipCourse> Frameworks { get; set; }
    }
}