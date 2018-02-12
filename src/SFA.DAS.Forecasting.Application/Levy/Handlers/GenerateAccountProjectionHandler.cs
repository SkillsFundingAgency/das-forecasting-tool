using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Shared;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Levy.Handlers
{
    public class GenerateAccountProjectionHandler
    {
        private readonly IAccountProjectionRepository _accountProjectionRepository;
        private readonly ILog _logger;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IPayrollDateService _payrollDateService;

        public GenerateAccountProjectionHandler(IAccountProjectionRepository accountProjectionRepository, ILog logger,
            IApplicationConfiguration applicationConfiguration, IPayrollDateService payrollDateService)
        {
            _accountProjectionRepository = accountProjectionRepository ?? throw new ArgumentNullException(nameof(accountProjectionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationConfiguration = applicationConfiguration ?? throw new ArgumentNullException(nameof(applicationConfiguration));
            _payrollDateService = payrollDateService ?? throw new ArgumentNullException(nameof(payrollDateService));
        }

        public async Task Handle(LevySchemeDeclarationUpdatedMessage levySchemeDeclaration)
        {
            _logger.Debug($"Now generating account projections for id: {levySchemeDeclaration.AccountId} in response to Levy event.");
            var accountProjections = await _accountProjectionRepository.Get(levySchemeDeclaration.AccountId);
            accountProjections.BuildLevyTriggeredProjections(DateTime.Today,_applicationConfiguration.NumberOfMonthsToProject);
            foreach (var accountProjectionReadModel in accountProjections.Projections)
            {
                _logger.Debug($"Generated projection: {accountProjectionReadModel.ToJson()}");
            }
            await _accountProjectionRepository.Store(accountProjections);
            _logger.Info($"Finished generating account projections for account: {levySchemeDeclaration.AccountId}.");
        }
    }
}