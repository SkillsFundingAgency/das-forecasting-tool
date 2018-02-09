using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.Levy.Services
{
    public class LevyEmployerDataService
    {
        private readonly IAccountApiClient _accountApiClient;
        private readonly IHashingService _hashingService;
        private readonly ILog _logger;

        public LevyEmployerDataService (
            IAccountApiClient accountApiClient, 
            IHashingService hashingService,
            ILog logger)
        {
            _accountApiClient = accountApiClient;
            _hashingService = hashingService;
            _logger = logger;
        }

        public async Task<LevyDeclarationUpdatedMessage> LevyForPeriod(string employerId, string periodYear, short? periodMonth)
        {
            _logger.Debug("1");
            // Getting by year month not available?
            var res = await _accountApiClient.GetLevyDeclarations(employerId);

            if (res == null)
                _logger.Debug("Res is null");

            var matches = res.Where(m => m.PayrollYear == periodYear && m.PayrollMonth == periodMonth);

            _logger.Debug($"3 {matches.Count()}");

            if (matches.Count() != 1)
                return null;

            _logger.Debug("4");
            var levy = matches.SingleOrDefault();
            var accountId = _hashingService.DecodeValue(levy.HashedAccountId);

            return new LevyDeclarationUpdatedMessage
            {
                AccountId = accountId,
                PayrollYear = levy.PayrollYear,
                PayrollMonth = levy.PayrollMonth,
                LevyDeclaredInMonth = levy.TotalAmount,
                CreatedAt = levy.CreatedDate
            };
        }
    }
}
