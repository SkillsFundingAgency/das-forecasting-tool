using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Application.Balance.Services
{
    public class AccountBalanceService : IAccountBalanceService
    {
        private readonly IAccountApiClient _accountApiClient;
        private readonly IHashingService _hashingService;
        private readonly ITelemetry _telemetry;

        public AccountBalanceService(IAccountApiClient accountApiClient,
            IHashingService hashingService, ITelemetry telemetry)
        {
            _accountApiClient = accountApiClient ?? throw new ArgumentNullException(nameof(accountApiClient));
            _hashingService = hashingService ?? throw new ArgumentNullException(nameof(hashingService));
            _telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task<Models.Balance.BalanceModel> GetAccountBalance(long accountId)
        {
            var startTime = DateTime.UtcNow;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var hashedAccountId = _hashingService.HashValue(accountId);
            var account = await _accountApiClient.GetAccount(hashedAccountId);
            stopwatch.Stop();
            _telemetry.TrackDependency(DependencyType.ApiCall, "GetAccountBalance", startTime, stopwatch.Elapsed, true);
            return new Models.Balance.BalanceModel
            {
                EmployerAccountId = accountId,
                RemainingTransferBalance = account.TransferAllowance,
                TransferAllowance = account.TransferAllowance,
                Amount = account.Balance
            };
        }
    }
}