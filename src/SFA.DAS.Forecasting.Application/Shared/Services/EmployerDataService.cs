using System.Collections.Generic;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.Forecasting.Application.Shared.Services;

public interface IEmployerDataService
{
    Task<List<LevySchemeDeclarationUpdatedMessage>> LevyForPeriod(long accountId, string payrollYear, short periodMonth);

    Task<List<long>> GetAllAccounts();

    Task<List<PeriodInformation>> GetPeriodIds();

    Task<List<long>> EmployersForPeriod(string payrollYear, short payrollMonth);
}

public class LevyDeclarations : List<LevyDeclarationViewModel> { }

public class EmployerDataService : IEmployerDataService
{
    private readonly ILogger<EmployerDataService> _logger;
    private readonly IEmployerDatabaseService _databaseService;

    public EmployerDataService(
        ILogger<EmployerDataService> logger,
        IEmployerDatabaseService databaseService
    )
    {
        _logger = logger;
        _databaseService = databaseService;
    }

    public async Task<List<LevySchemeDeclarationUpdatedMessage>> LevyForPeriod(long accountId, string payrollYear, short payrollMonth)
    {
        var levydeclarations = await _databaseService.GetAccountLevyDeclarations(accountId, payrollYear, payrollMonth);

        if (levydeclarations == null)
        {
            _logger.LogDebug($"Account API client returned null for GetLevyDeclarations for account {accountId}, period: {payrollYear}, {payrollMonth}");
            return null;
        }

        _logger.LogInformation($"Got {levydeclarations.Count} levy declarations for employer {accountId}.");
        var validLevyDeclarations = levydeclarations
            .OrderByDescending(ld => ld.SubmissionDate)
            .ToList();
        _logger.LogInformation($"Got {validLevyDeclarations.Count} levy declarations for period {payrollYear}, {payrollMonth} for employer {accountId}.");
            
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
            _logger.LogInformation("Not able to find any EmployerAccountIds");
            return new List<long>();
        }

        _logger.LogInformation($"Got {accountIds.Count}.");

        return accountIds.ToList();
    }

    public async Task<List<long>> GetAllAccounts()
    {
        var accountIds = await _databaseService.GetAccountIds();

        if (accountIds == null || !accountIds.Any())
        {
            _logger.LogInformation("Not able to find any EmployerAccountIds");
            return new List<long>();
        }

        _logger.LogInformation($"Got {accountIds.Count}.");

        return accountIds.ToList();
    }

    public async Task<List<PeriodInformation>> GetPeriodIds()
    {
        return await _databaseService.GetPeriodIds();
    }
}