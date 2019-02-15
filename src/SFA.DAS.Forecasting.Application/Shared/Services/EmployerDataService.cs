using System.Collections.Generic;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public interface IEmployerDataService
    {
        Task<List<LevySchemeDeclarationUpdatedMessage>> LevyForPeriod(string employerId, string payrollYear, short periodMonth);

        Task<List<long>> EmployersForPeriod();

	    Task<List<PeriodInformation>> GetPeriodIds();

        Task<List<long>> EmployersForPeriod(string payrollYear, short payrollMonth);
    }

    public class LevyDeclarations : List<LevyDeclarationViewModel>, IAccountResource { }

    public class EmployerDataService : IEmployerDataService
    {
        private readonly IHashingService _hashingService;
        private readonly ILog _logger;
        private readonly IEmployerDatabaseService _databaseService;

        public EmployerDataService(IHashingService hashingService,
            ILog logger,
            IEmployerDatabaseService databaseService
            )
        {
            _hashingService = hashingService;
            _logger = logger;
            _databaseService = databaseService;
        }

        public async Task<List<LevySchemeDeclarationUpdatedMessage>> LevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var levydeclarations = await _databaseService.GetAccountLevyDeclarations(accountId, payrollYear, payrollMonth);

            if (levydeclarations == null)
            {
                _logger.Debug($"Account API client returned null for GetLevyDeclarations for account {hashedAccountId}, period: {payrollYear}, {payrollMonth}");
                return null;
            }

            _logger.Info($"Got {levydeclarations.Count} levy declarations for employer {hashedAccountId}.");
            var validLevyDeclarations = levydeclarations
                .OrderByDescending(ld => ld.SubmissionDate)
                .ToList();
            _logger.Info($"Got {validLevyDeclarations.Count} levy declarations for period {payrollYear}, {payrollMonth} for employer {hashedAccountId}.");
            
            return validLevyDeclarations.Select(levy => new LevySchemeDeclarationUpdatedMessage
            {
                SubmissionId= levy.SubmissionId,
                AccountId = accountId,
                CreatedAt = levy.CreatedDate,
                CreatedDate = levy.CreatedDate,
                EmpRef = levy.EmpRef,
                PayrollMonth = levy.PayrollMonth,
                PayrollYear = levy.PayrollYear,
                LevyDeclaredInMonth = levy.Amount,
                SubmissionDate = levy.SubmissionDate,
            }).ToList();
        }

        public async Task<List<long>> EmployersForPeriod(string payrollYear, short payrollMonth)
        {
            var accountIds = await _databaseService.GetAccountIdsForPeriod(payrollYear, payrollMonth);
            if (accountIds == null || !accountIds.Any())
            {
                _logger.Info("Not able to find any EmployerAccountIds");
                return new List<long>();
            }

            _logger.Info($"Got {accountIds.Count}.");

            return accountIds.ToList();
        }

        public async Task<List<long>> EmployersForPeriod()
        {
            var accountIds = await _databaseService.GetAccountIds();

            if (accountIds == null || !accountIds.Any())
            {
                _logger.Info("Not able to find any EmployerAccountIds");
                return new List<long>();
            }

            _logger.Info($"Got {accountIds.Count}.");

            return accountIds.ToList();
        }

	    public async Task<List<PeriodInformation>> GetPeriodIds()
	    {
		    return await _databaseService.GetPeriodIds();
	    }
    }
}
