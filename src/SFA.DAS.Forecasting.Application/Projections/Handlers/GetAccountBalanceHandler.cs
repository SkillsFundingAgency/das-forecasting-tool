using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NLog;
using SFA.DAS.Forecasting.Application.Balance.Services;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Projections.Handlers
{
    public class GetAccountBalanceHandler
    {
        private readonly ICurrentBalanceRepository _currentBalanceRepository;
        private readonly ILog _logger;
        private readonly ITelemetry _telemetry;
        private readonly IAccountProjectionService _accountProjectionService;

        public GetAccountBalanceHandler(IAccountProjectionService accountProjectionService, ICurrentBalanceRepository currentBalanceRepository, ILog logger, ITelemetry telemetry)
        {
            _accountProjectionService = accountProjectionService;
            _currentBalanceRepository = currentBalanceRepository ?? throw new ArgumentNullException(nameof(currentBalanceRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task Handle(GenerateAccountProjectionCommand message)
        {
            _logger.Debug($"Getting balances for account: {message.EmployerAccountId}");
            _telemetry.AddEmployerAccountId(message.EmployerAccountId);
            var currentBalance = await _currentBalanceRepository.Get(message.EmployerAccountId);

            
            var refreshBalance = await _accountProjectionService.GetOriginalProjectionSource(message.EmployerAccountId,
                                     message.ProjectionSource) == ProjectionSource.PaymentPeriodEnd ? currentBalance.RefreshBalance(true, true) : currentBalance.RefreshBalance(true);

            if (!await refreshBalance)
            {
                _telemetry.TrackEvent("Account Balance Already Refreshed");
                _logger.Warn($"Failed to refresh the account balance for account {message.EmployerAccountId}.  It's possible the account has been refreshed recently.");
                return;
            }
            await _currentBalanceRepository.Store(currentBalance);
            _telemetry.TrackEvent("Refreshed Account Balance");
            _logger.Info($"Finished updating recorded balance for account: {message.EmployerAccountId}");
        }
    }
}