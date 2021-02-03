using SFA.DAS.Forecasting.Functions.Framework.Logging;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Forecasting.Functions.Framework.Infrastructure.Registries
{
    public class LoggingRegistry : Registry
    {
        public LoggingRegistry()
        {
            For<NLogLogger>().Use(NLogLoggerSetup.Create(null));
        }
    }
}