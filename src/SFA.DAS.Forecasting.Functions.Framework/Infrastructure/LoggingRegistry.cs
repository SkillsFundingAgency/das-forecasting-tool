using SFA.DAS.Forecasting.Functions.Framework.Logging;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.Forecasting.Functions.Framework.Infrastructure
{
    public class LoggingRegistry: Registry
    {
        public LoggingRegistry()
        {
           // ForSingletonOf<ILog>().Use(LoggerSetup.Create());
        }
    }
}