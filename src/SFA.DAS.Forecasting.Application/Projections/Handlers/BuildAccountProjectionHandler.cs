using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.ExpiredFunds.Service;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Projections.Handlers
{
    public class BuildAccountProjectionHandler
    {
        private readonly IAccountProjectionRepository _accountProjectionRepository;
        private readonly IAccountProjectionService _accountProjectionService;
		private readonly IExpiredFundsService _expiredFundsService;
        private readonly IApplicationConfiguration _config;
        private readonly ITelemetry _telemetry;
        private readonly ILog _logger;


        public BuildAccountProjectionHandler(IAccountProjectionRepository accountProjectionRepository,IAccountProjectionService accountProjectionService, IApplicationConfiguration config, ITelemetry telemetry, IExpiredFundsService expiredFundsService, ILog logger)
        {
            _accountProjectionRepository = accountProjectionRepository ?? throw new ArgumentNullException(nameof(accountProjectionRepository));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            _accountProjectionService = accountProjectionService ?? throw new ArgumentNullException(nameof(accountProjectionService));
			_expiredFundsService = expiredFundsService ?? throw new ArgumentNullException(nameof(expiredFundsService));
            _logger = logger;
        }

        public async Task Handle(GenerateAccountProjectionCommand message)
        {
            _telemetry.AddEmployerAccountId(message.EmployerAccountId);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var projections = await _accountProjectionRepository.InitialiseProjection(message.EmployerAccountId);

            var numberOfMonthsToProject = _config.NumberOfMonthsToProject;

            DateTime startDate;

            if (message.StartPeriod != null)
            {
                startDate = new DateTime(message.StartPeriod.Year, message.StartPeriod.Month, message.StartPeriod.Day);
            }
            else
            {
                startDate = DateTime.Today.GetStartOfAprilOfFinancialYear();

                var extraMonths = ((DateTime.Today.Year - startDate.Year) * 12) + DateTime.Today.Month - startDate.Month;
                numberOfMonthsToProject += extraMonths;
            }

            _logger.Info($"Projecting {numberOfMonthsToProject} months from {startDate:yyyy-MM-dd} for account {message.EmployerAccountId}");

            var messageProjectionSource = await _accountProjectionService.GetOriginalProjectionSource(message.EmployerAccountId,message.ProjectionSource);
           
            if (messageProjectionSource == ProjectionSource.LevyDeclaration)
                projections.BuildLevyTriggeredProjections(startDate, numberOfMonthsToProject);
            else
                projections.BuildPayrollPeriodEndTriggeredProjections(startDate, numberOfMonthsToProject);
    
            var expiringFunds = await _expiredFundsService.GetExpiringFunds(projections.Projections, message.EmployerAccountId, messageProjectionSource, startDate);

            if (expiringFunds.Any())
            {
                projections.UpdateProjectionsWithExpiredFunds(expiringFunds);
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