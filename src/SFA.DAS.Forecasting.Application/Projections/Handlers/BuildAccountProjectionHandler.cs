using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.ExpiredFunds.Service;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Application.Projections.Handlers
{
    public class BuildAccountProjectionHandler
    {
        private readonly IAccountProjectionRepository _accountProjectionRepository;
        private readonly IExpiredFundsService _expiredFundsService;
        private readonly ILevyDataSession _levyDataSession;
        private readonly IApplicationConfiguration _config;
        private readonly ITelemetry _telemetry;



        public BuildAccountProjectionHandler(IAccountProjectionRepository accountProjectionRepository, IApplicationConfiguration config, ITelemetry telemetry, IExpiredFundsService expiredFundsService, ILevyDataSession levyDataSession)
        {
            _accountProjectionRepository = accountProjectionRepository ?? throw new ArgumentNullException(nameof(accountProjectionRepository));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            _expiredFundsService = expiredFundsService ?? throw new ArgumentNullException(nameof(expiredFundsService));
            _levyDataSession = levyDataSession ?? throw new ArgumentNullException(nameof(levyDataSession)); ;
        }

        public async Task Handle(GenerateAccountProjectionCommand message)
        {
            _telemetry.AddEmployerAccountId(message.EmployerAccountId);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var projections = await _accountProjectionRepository.InitialiseProjection(message.EmployerAccountId);
            var startDate = new DateTime(
                GetValue(message.StartPeriod?.Year, DateTime.Today.Year),
                GetValue(message.StartPeriod?.Month, DateTime.Today.Month),
                1);

            if (message.ProjectionSource == ProjectionSource.LevyDeclaration)
                projections.BuildLevyTriggeredProjections(startDate, _config.NumberOfMonthsToProject);
            else
                projections.BuildPayrollPeriodEndTriggeredProjections(startDate, _config.NumberOfMonthsToProject);

            if (_config.FeatureExpiredFunds)
            {
                var expiringFunds = _expiredFundsService.GetExpiringFunds(projections.Projections, message.EmployerAccountId);

                projections.UpdateProjectionsWithExpiredFunds(await expiringFunds);
            }
            
            await _accountProjectionRepository.Store(projections);
            
            stopwatch.Stop();
            _telemetry.TrackDuration("BuildAccountProjection", stopwatch.Elapsed);
        }

        private int GetValue(int? value, int defaultValue)
        {
            return value.HasValue && value.Value > 0 ? value.Value : defaultValue;
        }
    }
}