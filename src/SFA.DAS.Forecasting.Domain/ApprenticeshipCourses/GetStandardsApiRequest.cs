namespace SFA.DAS.Forecasting.Domain.ApprenticeshipCourses
{
    public class GetStandardsApiRequest :IGetApiRequest
    {
        public GetStandardsApiRequest(string baseUrl)
        {
            BaseUrl = baseUrl;
        }
        public string BaseUrl { get; }
        public string GetUrl => $"{BaseUrl}standards";
    }
}