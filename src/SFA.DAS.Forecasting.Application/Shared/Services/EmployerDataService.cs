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
        Task<List<LevySchemeDeclarationUpdatedMessage>> LevyForPeriod(string employerId, string payrollYear, short? periodMonth);
    }

    public class LevyDeclarations: List<LevyDeclarationViewModel>, IAccountResource { }

    public class EmployerDataService : IEmployerDataService
    {
        private readonly IAccountApiClient _accountApiClient;
        private readonly IHashingService _hashingService;
        private readonly ILog _logger;

        public EmployerDataService(
            IAccountApiClient accountApiClient,
            IHashingService hashingService,
            ILog logger)
        {
            _accountApiClient = accountApiClient;
            _hashingService = hashingService;
            _logger = logger;
        }

        public async Task<List<LevySchemeDeclarationUpdatedMessage>> LevyForPeriod(string hashedAccountId, string payrollYear, short? payrollMonth)
        {
            var levydeclarations = await _accountApiClient.GetLevyDeclarations(hashedAccountId);

            if (levydeclarations == null)
            {
                _logger.Debug($"Account API client returned null for GetLevyDeclarations for account {hashedAccountId}, period: {payrollYear}, {payrollMonth}");
                return null;
            }

            _logger.Info($"Got {levydeclarations.Count} levy declarations for employer {hashedAccountId}.");
            var validLevyDeclarations = levydeclarations.Where(levy => levy.PayrollYear == payrollYear && levy.PayrollMonth == payrollMonth).ToList();
            _logger.Info($"Got {validLevyDeclarations.Count} levy declarations for period {payrollYear}, {payrollMonth} for employer {hashedAccountId}.");
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            return validLevyDeclarations.Select(levy => new LevySchemeDeclarationUpdatedMessage
            {
                Id = levy.Id,
                AccountId = accountId,
                CreatedAt = levy.CreatedDate,
                CreatedDate = levy.CreatedDate,
                DateCeased = levy.DateCeased,
                EmpRef = levy.PayeSchemeReference,
                PayrollMonth = levy.PayrollMonth,
                PayrollYear = levy.PayrollYear,
                LevyDeclaredInMonth = levy.LevyDeclaredInMonth,
                LevyAllowanceForYear = levy.LevyAllowanceForYear ?? 0,
                SubmissionDate = levy.SubmissionDate ?? DateTime.MinValue,
                TotalAmount = levy.TotalAmount
            }).ToList();
        }
    }
}
