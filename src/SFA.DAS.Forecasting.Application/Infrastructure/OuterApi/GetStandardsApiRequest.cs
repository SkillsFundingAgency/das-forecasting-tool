namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi
{
    public class GetStandardsApiRequest :IGetApiRequest
    {
        public string GetUrl => $"standards";
    }
}