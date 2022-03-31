using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi
{
    public class GetAccountIdsApiRequest : IGetApiRequest
    {
        public string GetUrl => "approvals/accountIds";
    }

    public class GetAccountIdsResponse
    {
        public List<long> AccountIds { get; set; }
    }
}
