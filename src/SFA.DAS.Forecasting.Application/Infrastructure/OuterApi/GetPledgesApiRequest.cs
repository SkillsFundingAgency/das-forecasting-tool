using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;

public class GetPledgesApiRequest : IGetApiRequest
{
    public GetPledgesApiRequest(long accountId)
    {
        AccountId = accountId;
    }

    public long AccountId { get; }

    public string GetUrl => $"pledges?accountId={AccountId}";
}

public class GetPledgesResponse
{
    public int TotalPledges { get; set; }

    public List<Pledge> Pledges { get; set; }


    public class Pledge
    {
        public int Id { get; set; }
        public long AccountId { get; set; }
    }
}