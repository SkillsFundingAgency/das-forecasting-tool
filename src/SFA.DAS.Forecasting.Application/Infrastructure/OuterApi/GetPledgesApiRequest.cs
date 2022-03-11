namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi
{
    public class GetPledgesApiRequest : IGetApiRequest
    {
        public string GetUrl => "pledges";
    }

    public class GetPledgesResponse
    {
        public int TotalPledges { get; set; }
    }
}
