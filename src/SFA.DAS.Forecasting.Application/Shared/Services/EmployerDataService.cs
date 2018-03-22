using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Account.Api.Client;
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
    }

    public class LevyDeclarations: List<LevyDeclarationViewModel>, IAccountResource { }

    public class EmployerDataService : IEmployerDataService
    {
        private readonly IAccountApiClient _accountApiClient;
        private readonly IHashingService _hashingService;
        private readonly ILog _logger;
        private readonly IEmployerDatabaseService _databaseService;

        public EmployerDataService(
            IAccountApiClient accountApiClient,
            IHashingService hashingService,
            ILog logger, 
            IEmployerDatabaseService databaseService
            )
        {
            _accountApiClient = accountApiClient;
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
            var validLevyDeclarations = levydeclarations.Where(levy => levy.PayrollYear == payrollYear && levy.PayrollMonth == payrollMonth).ToList();
            _logger.Info($"Got {validLevyDeclarations.Count} levy declarations for period {payrollYear}, {payrollMonth} for employer {hashedAccountId}.");
            
            return validLevyDeclarations.Select(levy => new LevySchemeDeclarationUpdatedMessage
            {
                Id = levy.Id,
                AccountId = accountId,
                CreatedAt = levy.CreatedDate,
                CreatedDate = levy.CreatedDate,
                DateCeased = levy.DateCeased,
                EmpRef = levy.EmpRef,
                PayrollMonth = levy.PayrollMonth,
                PayrollYear = levy.PayrollYear,
                LevyDeclaredInMonth = levy.LevyDeclaredInMonth,
                LevyAllowanceForYear = levy.LevyAllowanceForYear,
                SubmissionDate = levy.SubmissionDate,
                TotalAmount = levy.TotalAmount
            }).ToList();
        }
    }
}
