using SFA.DAS.Forecasting.Levy.Domain;
using SFA.DAS.NLog.Logger;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Levy.Application
{
    public class LevyWorker : ILevyWorker
    {
        private readonly ILog _logger;

        public LevyWorker(ILog logger)
        {
            _logger = logger;
        }

        public async Task Run()
        {
            _logger.Info("This is the Levy Worker running");
            await Task.Run(() => 1);
        }
    }
}
