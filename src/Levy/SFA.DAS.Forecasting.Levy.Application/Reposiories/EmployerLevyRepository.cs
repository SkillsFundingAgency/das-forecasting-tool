using SFA.DAS.Forecasting.Levy.Domain.Entities;
using SFA.DAS.Forecasting.Levy.Domain.Repositories;
using SFA.DAS.NLog.Logger;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Levy.Application.Reposiories
{
    public class EmployerLevyRepository : IEmployerLevyRepository
    {
        private readonly ILog _logger;

        public EmployerLevyRepository(ILog logger)
        {
            _logger = logger;
        }

        public async Task StoreLevyDeclaration(LevyDeclaration levyDeclaration)
        {

            _logger.Info("Store event");
        }
    }
}
