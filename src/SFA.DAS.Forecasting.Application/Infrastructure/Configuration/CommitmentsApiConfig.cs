using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Http;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public class CommitmentsApiConfig : ICommitmentsApiClientConfiguration, IJwtClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
    }
}