using System;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;

public class GetApplicationsApiRequest : IGetApiRequest
{
    public int PledgeId { get;  }

    public GetApplicationsApiRequest(int pledgeId)
    {
        PledgeId = pledgeId;
    }

    public string GetUrl => $"applications?pledgeId={PledgeId}";
}

public class GetApplicationsResponse
{
    public List<Application> Applications { get; set; }

    public class Application
    {
        public int Id { get; set; }
        public int PledgeId { get; set; }
        public long EmployerAccountId { get; set; }
        public string StandardId { get; set; }
        public string StandardTitle { get; set; }
        public int StandardLevel { get; set; }
        public int StandardDuration { get; set; }
        public int StandardMaxFunding { get; set; }
        public DateTime StartDate { get; set; }
        public int NumberOfApprentices { get; set; }
        public int NumberOfApprenticesUsed { get; set; }
        public string Status { get; set; }
    }
}