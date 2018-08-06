using SFA.DAS.Commitments.Api.Client.Configuration;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public class CommitmentsApiConfig : ICommitmentsApiClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
    }
}