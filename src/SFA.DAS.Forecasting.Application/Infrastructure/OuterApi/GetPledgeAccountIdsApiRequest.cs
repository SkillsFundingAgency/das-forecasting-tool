using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;

public class GetPledgeAccountIdsApiRequest : IGetApiRequest
{
    public string GetUrl => "pledges/accountIds";
}

public class GetPledgeAccountIdsResponse
{
    public List<long> AccountIds { get; set; }
}