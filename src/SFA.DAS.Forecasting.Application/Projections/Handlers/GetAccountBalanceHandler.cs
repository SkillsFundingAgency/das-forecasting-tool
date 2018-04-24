using System;
using System.Threading.Tasks;
using NLog;
using SFA.DAS.Forecasting.Application.Balance.Services;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Projections.Handlers
{
    public class GetAccountBalanceHandler
    {
        private readonly IAccountBalanceService _accountBalanceService;
        private readonly ICurrentBalanceRepository _currentBalanceRepository;
        private readonly ILog _logger;

        public GetAccountBalanceHandler(IAccountBalanceService accountBalanceService, ICurrentBalanceRepository currentBalanceRepository, ILog logger)
        {
            _accountBalanceService = accountBalanceService ?? throw new ArgumentNullException(nameof(accountBalanceService));
            _currentBalanceRepository = currentBalanceRepository ?? throw new ArgumentNullException(nameof(currentBalanceRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(GenerateAccountProjectionCommand message)
        {
            _logger.Debug($"Getting balances for account: {message.EmployerAccountId}");
            var currentBalance = await _currentBalanceRepository.Get(message.EmployerAccountId);
            if (!await currentBalance.RefreshBalance())
            {
                _logger.Warn($"Failed to refresh the account balance for account {message.EmployerAccountId}.  It's possible the account has been refreshed recently.");
                return;
            }
            await _currentBalanceRepository.Store(currentBalance);
            _logger.Info($"Finished updating recorded balance for account: {message.EmployerAccountId}");
        }
    }
}