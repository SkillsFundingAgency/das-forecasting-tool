using SFA.DAS.Commitments.Api.Client.Configuration;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Configuration
{
    public class CommitmentsApiConfig : ICommitmentsApiClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
        public string ApiBaseUrl { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
    }
}