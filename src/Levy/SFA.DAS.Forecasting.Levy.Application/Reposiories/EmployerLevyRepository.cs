using SFA.DAS.Forecasting.Levy.Domain.Entities;
using SFA.DAS.Forecasting.Levy.Domain.Repositories;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Levy.Application.Reposiories
{
    public class EmployerLevyRepository : IEmployerLevyRepository
    {
        private readonly ILog _logger;

        public EmployerLevyRepository(ILog logger)
        {
            _logger = logger;
        }

        public void StoreLevyDeclaration(LevyDeclaration levyDeclaration)
        {
            _logger.Info("Store event");
        }
    }
}
