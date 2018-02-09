using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Forecasting.Application.Levy.Messages;
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

        public async Task<LevySchemeDeclarationUpdatedMessage> LevyForPeriod(string employerId, string periodYear, short? periodMonth)
        {
            _logger.Debug("1");
            var res = await _accountApiClient.GetLevyDeclarations(employerId);

            if (res == null)
            {
                _logger.Debug($"Account API client returned null");
                return null;
            }

            var matches = res.Where(m => m.PayrollYear == periodYear && m.PayrollMonth == periodMonth);

            if (matches.Count() != 1)
            {
                _logger.Info($"Can't find any declarations for {employerId}, Year: {periodYear}, Month: {periodMonth}");
                return null;
            }

            var levy = matches.SingleOrDefault();
            var accountId = _hashingService.DecodeValue(levy.HashedAccountId);

            return new LevySchemeDeclarationUpdatedMessage
            {
                AccountId = accountId,
                EmpRef = employerId,
                PayrollYear = levy.PayrollYear,
                PayrollMonth = levy.PayrollMonth,
                LevyDeclaredInMonth = levy.TotalAmount,
                CreatedDate = levy.CreatedDate
            };
        }
    }
}
