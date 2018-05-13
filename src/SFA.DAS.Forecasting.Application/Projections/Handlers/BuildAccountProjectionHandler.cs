using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Application.Projections.Handlers
{
    public class BuildAccountProjectionHandler
    {
        private readonly IAccountProjectionRepository _accountProjectionRepository;
        private readonly IApplicationConfiguration _config;


        public BuildAccountProjectionHandler(IAccountProjectionRepository accountProjectionRepository, IApplicationConfiguration config)
        {
            _accountProjectionRepository = accountProjectionRepository ?? throw new ArgumentNullException(nameof(accountProjectionRepository));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task Handle(GenerateAccountProjectionCommand message)
        {
            var projections = await _accountProjectionRepository.Get(message.EmployerAccountId);
            var startDate = new DateTime(GetValue(message.StartPeriod?.Year, DateTime.Today.Year),
                GetValue(message.StartPeriod?.Month, DateTime.Today.Month), 1);
            if (message.ProjectionSource == ProjectionSource.LevyDeclaration)
                projections.BuildLevyTriggeredProjections(startDate, _config.NumberOfMonthsToProject);
            else
                projections.BuildPayrollPeriodEndTriggeredProjections(startDate, _config.NumberOfMonthsToProject);
            await _accountProjectionRepository.Store(projections);
        }

        private int GetValue(int? value, int defaultValue)
        {
            return value.HasValue && value.Value > 0 ? value.Value : defaultValue;
        }
    }
}