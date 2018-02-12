using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Application.Projections.Handlers
{
    public class GenerateLevyAccountProjectionHandler
    {
        private readonly IAccountProjectionRepository _accountProjectionRepository;
        private readonly IApplicationConfiguration _config;


        public GenerateLevyAccountProjectionHandler(IAccountProjectionRepository accountProjectionRepository, IApplicationConfiguration config)
        {
            _accountProjectionRepository = accountProjectionRepository ?? throw new ArgumentNullException(nameof(accountProjectionRepository));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task Handle(GenerateLevyAccountProjection message)
        {
            var projections = await _accountProjectionRepository.Get(message.EmployerAccountId);
            projections.BuildLevyTriggeredProjections(DateTime.Today, _config.NumberOfMonthsToProject);
            await _accountProjectionRepository.Store(projections);
        }
    }
}