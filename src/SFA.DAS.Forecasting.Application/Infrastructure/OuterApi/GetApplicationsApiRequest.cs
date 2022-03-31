namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi
{
    public class GetApplicationsApiRequest : IGetApiRequest
    {
        public string GetUrl => "applications";
    }

    public class GetApplicationsResponse
    {
        public int TotalApplications { get; set; }
    }
}
