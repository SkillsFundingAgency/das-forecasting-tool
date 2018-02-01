using SFA.DAS.Forecasting.Domain.Interfaces;

namespace SFA.DAS.Forecasting.Infrastructure.Configuration
{
    public class ForcastingApplicationConfiguration : IApplicationConfiguration
    {
        public string DatabaseConnectionString { get; set; }

        public string AllowedHashstringCharacters { get; set; }

        public string HashString { get; set; }

        public string BackLink { get; set; }
    }
}