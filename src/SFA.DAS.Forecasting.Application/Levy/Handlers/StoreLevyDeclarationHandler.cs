using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Domain.Levy;
using SFA.DAS.Forecasting.Models.Levy;


namespace SFA.DAS.Forecasting.Application.Levy.Handlers;

public interface IStoreLevyDeclarationHandler
{
    Task Handle(LevySchemeDeclarationUpdatedMessage levySchemeDeclaration, string allowProjectionsEndpoint);
}
public class StoreLevyDeclarationHandler : IStoreLevyDeclarationHandler
{
    private readonly ILogger<StoreLevyDeclarationHandler> _logger;
    private readonly IQueueService _queueService;
    public ILevyDeclarationRepository Repository { get; }

    public StoreLevyDeclarationHandler(ILevyDeclarationRepository repository, ILogger<StoreLevyDeclarationHandler> logger, IQueueService queueService)
    {
        _logger = logger;
        _queueService = queueService;
        Repository = repository;
    }

    public async Task Handle(LevySchemeDeclarationUpdatedMessage levySchemeDeclaration, string allowProjectionsEndpoint)
    {
        _logger.LogDebug($"Now handling the levy declaration event: {levySchemeDeclaration.AccountId}, {levySchemeDeclaration.EmpRef}");
        if (levySchemeDeclaration.PayrollMonth == null)
            throw new InvalidOperationException($"Received invalid levy declaration. No month specified. Data: ");

        var declaration = MapLevySchemeDeclarationUpdatedMessageToLevyDeclarationModel(levySchemeDeclaration);

        var levyDeclaration = await Repository.Get(declaration);
        _logger.LogDebug("Now adding levy declaration to levy period.");
        levyDeclaration.RegisterLevyDeclaration(levySchemeDeclaration.LevyDeclaredInMonth, levySchemeDeclaration.CreatedDate);
        _logger.LogDebug($"Now storing the levy period. Employer: {levySchemeDeclaration.AccountId}, year: {levySchemeDeclaration.PayrollYear}, month: {levySchemeDeclaration.PayrollMonth}");
        await Repository.Store(levyDeclaration);
        _logger.LogDebug($"Finished adding the levy declaration to the levy period. Levy declaration: {levyDeclaration.Id}");

        if (!string.IsNullOrWhiteSpace(allowProjectionsEndpoint))
        {
            _queueService.SendMessageWithVisibilityDelay(levySchemeDeclaration, allowProjectionsEndpoint);
        }
    }

    private static LevyDeclarationModel MapLevySchemeDeclarationUpdatedMessageToLevyDeclarationModel(
        LevySchemeDeclarationUpdatedMessage levySchemeDeclaration)
    {
        var declaration = new LevyDeclarationModel
        {
            EmployerAccountId = levySchemeDeclaration.AccountId,
            TransactionDate = levySchemeDeclaration.CreatedDate,
            LevyAmountDeclared = levySchemeDeclaration.LevyDeclaredInMonth,
            SubmissionId = levySchemeDeclaration.SubmissionId,
            PayrollMonth = (byte)levySchemeDeclaration.PayrollMonth,
            PayrollYear = levySchemeDeclaration.PayrollYear,
            Scheme = levySchemeDeclaration.EmpRef
        };
        return declaration;
    }
}