using System;

using SFA.DAS.Forecasting.Domain.Interfaces;

namespace SFA.DAS.Forecasting.Infrastructure.Configuration
{
    public class ForcastingConfiguration : IConfiguration
    {
        public string DatabaseConnectionString
        {
            get
            {
                return "Data Source=(localdb)\\ProjectsV13;Initial Catalog=SFA.DAS.Forecasting.Database;Integrated Security=True;Pooling=False;Connect Timeout=30";
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
            }
        }
    }
}