namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi
{
    public class GetFrameworksApiRequest :IGetApiRequest
    {
        public string GetUrl => $"frameworks";
    }
}