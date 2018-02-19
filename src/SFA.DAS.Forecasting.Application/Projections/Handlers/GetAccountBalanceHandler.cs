using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Balance.Services;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Application.Projections.Handlers
{
    public class GetAccountBalanceHandler
    {
        private readonly IAccountBalanceService _accountBalanceService;
        private readonly ICurrentBalanceRepository _currentBalanceRepository;

        public GetAccountBalanceHandler(IAccountBalanceService accountBalanceService, ICurrentBalanceRepository currentBalanceRepository)
        {
            _accountBalanceService = accountBalanceService ?? throw new ArgumentNullException(nameof(accountBalanceService));
            _currentBalanceRepository = currentBalanceRepository ?? throw new ArgumentNullException(nameof(currentBalanceRepository));
        }

        public async Task Handle(GenerateAccountProjectionCommand message)
        {
            var balance = await _accountBalanceService.GetAccountBalance(message.EmployerAccountId);
            var currentBalance = await _currentBalanceRepository.Get(message.EmployerAccountId);
            currentBalance.SetCurrentBalance(balance, DateTime.UtcNow);
            await _currentBalanceRepository.Store(currentBalance);
        }
    }
}