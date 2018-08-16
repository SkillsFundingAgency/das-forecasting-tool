using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NLog;
using SFA.DAS.Forecasting.Application.Balance.Services;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
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

        public GetAccountBalanceHandler(IAccountBalanceService accountBalanceService, ICurrentBalanceRepository currentBalanceRepository, ILog logger, ITelemetry telemetry)
        {
            _currentBalanceRepository = currentBalanceRepository ?? throw new ArgumentNullException(nameof(currentBalanceRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task Handle(GenerateAccountProjectionCommand message)
        {
            _logger.Debug($"Getting balances for account: {message.EmployerAccountId}");
            _telemetry.AddEmployerAccountId(message.EmployerAccountId);
            var currentBalance = await _currentBalanceRepository.Get(message.EmployerAccountId);
            if (!await currentBalance.RefreshBalance(true))
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