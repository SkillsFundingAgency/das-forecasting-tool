using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Projections.Services;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Application.Projections.Handlers
{
    public interface IGetAccountBalanceHandler
    {
        Task Handle(GenerateAccountProjectionCommand message);
    }
    public class GetAccountBalanceHandler : IGetAccountBalanceHandler
    {
        private readonly ICurrentBalanceRepository _currentBalanceRepository;
        private readonly ILogger<GetAccountBalanceHandler> _logger;
        private readonly IAccountProjectionService _accountProjectionService;

        public GetAccountBalanceHandler(IAccountProjectionService accountProjectionService, ICurrentBalanceRepository currentBalanceRepository, ILogger<GetAccountBalanceHandler> logger)
        {
            _accountProjectionService = accountProjectionService;
            _currentBalanceRepository = currentBalanceRepository ?? throw new ArgumentNullException(nameof(currentBalanceRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(GenerateAccountProjectionCommand message)
        {
            _logger.LogDebug($"Getting balances for account: {message.EmployerAccountId}");
            
            var currentBalance = await _currentBalanceRepository.Get(message.EmployerAccountId);

            
            var refreshBalance = await _accountProjectionService.GetOriginalProjectionSource(message.EmployerAccountId,
                                     message.ProjectionSource) == ProjectionSource.PaymentPeriodEnd ? currentBalance.RefreshBalance(true, true) : currentBalance.RefreshBalance(true);

            if (!await refreshBalance)
            {
                 _logger.LogWarning($"Failed to refresh the account balance for account {message.EmployerAccountId}.  It's possible the account has been refreshed recently.");
                return;
            }
            await _currentBalanceRepository.Store(currentBalance);
            _logger.LogInformation($"Finished updating recorded balance for account: {message.EmployerAccountId}");
        }
    }
}