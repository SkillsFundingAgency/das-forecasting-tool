using Microsoft.Practices.ObjectBuilder2;

namespace SFA.DAS.Forecasting.Domain.ApprenticeshipCourses
{
    public class GetFrameworksApiRequest :IGetApiRequest
    {
        public GetFrameworksApiRequest(string baseUrl)
        {
            BaseUrl = baseUrl;
        }
        public string BaseUrl { get;}
        public string GetUrl => $"{BaseUrl}frameworks";
    }
}