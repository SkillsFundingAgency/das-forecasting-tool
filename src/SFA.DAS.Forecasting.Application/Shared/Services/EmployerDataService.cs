using System.Collections.Generic;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.NLog.Logger;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public interface IEmployerDataService
    {
        Task<List<LevySchemeDeclarationUpdatedMessage>> LevyForPeriod(long employerAccountId, string payrollYear, short periodMonth);

        Task<List<long>> EmployersForPeriod(string payrollYear, short payrollMonth);
    }

    public class LevyDeclarations : List<LevyDeclarationViewModel>, IAccountResource { }

    public class EmployerDataService : IEmployerDataService
    {
        private readonly ILog _logger;
        private readonly IEmployerDatabaseService _databaseService;

        public EmployerDataService(ILog logger, IEmployerDatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
        }

        public async Task<List<LevySchemeDeclarationUpdatedMessage>> LevyForPeriod(long employerAccountId, string payrollYear, short payrollMonth)
        {
            
            var levyDeclarations = await _databaseService.GetAccountLevyDeclarations(employerAccountId, payrollYear, payrollMonth);

            if (levyDeclarations == null)
            {
                _logger.Debug($"Account API client returned null for GetLevyDeclarations for account {employerAccountId}, period: {payrollYear}, {payrollMonth}");
                return null;
            }

            _logger.Info($"Got {levyDeclarations.Count} levy declarations for employer {employerAccountId}.");
            var validLevyDeclarations = levyDeclarations.Where(levy => levy.PayrollYear == payrollYear && levy.PayrollMonth == payrollMonth).ToList();
            validLevyDeclarations = validLevyDeclarations
                .GroupBy(ld => ld.EmpRef)
                .Select(g => g.OrderByDescending(ld => ld.SubmissionDate).FirstOrDefault())
                .ToList();
            _logger.Info($"Got {validLevyDeclarations.Count} levy declarations for period {payrollYear}, {payrollMonth} for employer {employerAccountId}.");

            return validLevyDeclarations.Select(levy => new LevySchemeDeclarationUpdatedMessage
            {
                Id = levy.Id,
                AccountId = employerAccountId,
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
            var accountIds = await _databaseService
                .GetAccountIds(payrollYear, payrollMonth);

            if (accountIds == null || !accountIds.Any())
            {
                _logger.Info($"Not able to find any EmployerAccountIds, Year: {payrollYear}, Month: {payrollMonth}");
                return new List<long>();
            }

            _logger.Info($"Got {accountIds.Count} for Year: {payrollYear} Month: {payrollMonth}.");

            return accountIds.ToList();
        }
    }
}
