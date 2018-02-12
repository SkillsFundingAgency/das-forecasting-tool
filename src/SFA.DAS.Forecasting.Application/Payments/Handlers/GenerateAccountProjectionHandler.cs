using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Payments.Handlers
{
    public class GenerateAccountProjectionHandler
    {
        private readonly IAccountProjectionRepository _accountProjectionRepository;
        private readonly ILog _logger;
        private readonly IApplicationConfiguration _applicationConfiguration;

        public GenerateAccountProjectionHandler(IAccountProjectionRepository accountProjectionRepository, ILog logger, IApplicationConfiguration applicationConfiguration)
        {
            _accountProjectionRepository = accountProjectionRepository ?? throw new ArgumentNullException(nameof(accountProjectionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationConfiguration = applicationConfiguration ?? throw new ArgumentNullException(nameof(applicationConfiguration));
        }

        public async Task Handle(PaymentCreatedMessage paymentCreatedMessage)
        {
            _logger.Debug($"Now generating account projections for id: {paymentCreatedMessage.EmployerAccountId} in response to Levy event.");
            var accountProjections = await _accountProjectionRepository.Get(paymentCreatedMessage.EmployerAccountId);
            accountProjections.BuildPayrollPeriodEndTriggeredProjections(DateTime.Today, _applicationConfiguration.NumberOfMonthsToProject);
            foreach (var accountProjectionReadModel in accountProjections.Projections)
            {
                _logger.Debug($"Generated projection: {accountProjectionReadModel.ToJson()}");
            }
            await _accountProjectionRepository.Store(accountProjections);
            _logger.Info($"Finished generating account projections for account: {paymentCreatedMessage.EmployerAccountId}.");
        }
    }
}