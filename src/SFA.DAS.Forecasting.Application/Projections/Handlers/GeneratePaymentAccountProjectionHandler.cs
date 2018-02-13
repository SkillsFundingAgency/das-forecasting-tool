using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Application.Projections.Handlers
{
    public class GeneratePaymentAccountProjectionHandler
    {
        private readonly IAccountProjectionRepository _accountProjectionRepository;
        private readonly IApplicationConfiguration _config;


        public GeneratePaymentAccountProjectionHandler(IAccountProjectionRepository accountProjectionRepository, IApplicationConfiguration config)
        {
            _accountProjectionRepository = accountProjectionRepository ?? throw new ArgumentNullException(nameof(accountProjectionRepository));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task Handle(GeneratePaymentAccountProjection message)
        {
            var projections = await _accountProjectionRepository.Get(message.EmployerAccountId);
            projections.BuildPayrollPeriodEndTriggeredProjections(DateTime.Today, _config.NumberOfMonthsToProject);
            await _accountProjectionRepository.Store(projections);
        }
    }
}