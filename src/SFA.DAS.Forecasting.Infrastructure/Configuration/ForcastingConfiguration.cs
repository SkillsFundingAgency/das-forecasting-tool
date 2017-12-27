using System;

using SFA.DAS.Forecasting.Domain.Interfaces;

namespace SFA.DAS.Forecasting.Infrastructure.Configuration
{
    public class ForcastingApplicationConfiguration : IApplicationConfiguration
    {
        public string DatabaseConnectionString { get; set; }

        public HashingServieConfig HashingService { get; set; }
    }

    public class HashingServieConfig
    {
        public string AllowedCharacters { get; set; }

        public string Hashstring { get; set; }
    }
}