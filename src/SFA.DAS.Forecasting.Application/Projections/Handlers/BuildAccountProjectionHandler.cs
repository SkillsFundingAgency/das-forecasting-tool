using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Core;
using SFA.DAS.Forecasting.Application.ExpiredFunds.Service;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Domain.Extensions;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Application.Projections.Handlers
{
    public interface IBuildAccountProjectionHandler
    {
        Task Handle(GenerateAccountProjectionCommand message);
    }
    public class BuildAccountProjectionHandler : IBuildAccountProjectionHandler
    {
        private readonly IAccountProjectionRepository _accountProjectionRepository;
        private readonly IAccountProjectionService _accountProjectionService;
		private readonly IExpiredFundsService _expiredFundsService;
        private readonly ForecastingJobsConfiguration _config;
        private readonly ILogger<BuildAccountProjectionHandler> _logger;


        public BuildAccountProjectionHandler(IAccountProjectionRepository accountProjectionRepository,IAccountProjectionService accountProjectionService, ForecastingJobsConfiguration config, IExpiredFundsService expiredFundsService, ILogger<BuildAccountProjectionHandler> logger)
        {
            _accountProjectionRepository = accountProjectionRepository ?? throw new ArgumentNullException(nameof(accountProjectionRepository));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _accountProjectionService = accountProjectionService ?? throw new ArgumentNullException(nameof(accountProjectionService));
			_expiredFundsService = expiredFundsService ?? throw new ArgumentNullException(nameof(expiredFundsService));
            _logger = logger;
        }

        public async Task Handle(GenerateAccountProjectionCommand message)
        {
            var projections = await _accountProjectionRepository.InitialiseProjection(message.EmployerAccountId);

            var numberOfMonthsToProject = ForecastingJobsConfiguration.NumberOfMonthsToProject;

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

            _logger.LogInformation($"Projecting {numberOfMonthsToProject} months from {startDate:yyyy-MM-dd} for account {message.EmployerAccountId}");

            var messageProjectionSource = await _accountProjectionService.GetOriginalProjectionSource(message.EmployerAccountId,message.ProjectionSource);
           
            if (messageProjectionSource == ProjectionSource.LevyDeclaration)
                projections.BuildLevyTriggeredProjections(startDate, numberOfMonthsToProject, DateTime.UtcNow);
            else
                projections.BuildPayrollPeriodEndTriggeredProjections(startDate, numberOfMonthsToProject, DateTime.UtcNow);
    
            var expiringFunds = await _expiredFundsService.GetExpiringFunds(projections.Projections, message.EmployerAccountId, messageProjectionSource, startDate);

            if (expiringFunds.Any())
            {
                projections.UpdateProjectionsWithExpiredFunds(expiringFunds);
            }
            
            await _accountProjectionRepository.Store(projections);
            
        }
    }
}