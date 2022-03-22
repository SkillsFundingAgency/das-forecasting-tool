using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi
{
    public class GetPledgesApiRequest : IGetApiRequest
    {
        public string GetUrl => "pledges";
    }

    public class GetPledgesResponse
    {
        public int TotalPledges { get; set; }

        public List<Pledge> Pledges { get; set; }


        public class Pledge
        {
            public long AccountId { get; set; }
        }
    }
}
