using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Levy;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Levy.Handlers
{
    public class StoreLevyDeclarationHandler
    {
        private readonly IQueueService _queueService;
        public ILevyDeclarationRepository Repository { get; }
        public ILog Logger { get; }

        public StoreLevyDeclarationHandler(ILevyDeclarationRepository repository, ILog logger, IQueueService queueService)
        {
            _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(LevySchemeDeclarationUpdatedMessage levySchemeDeclaration, string allowProjectionsEndpoint)
        {
            Logger.Debug($"Now handling the levy declaration event: {levySchemeDeclaration.AccountId}, {levySchemeDeclaration.EmpRef}");
            if (levySchemeDeclaration.PayrollMonth == null)
                throw new InvalidOperationException($"Received invalid levy declaration. No month specified. Data: ");
            var levyDeclaration = await Repository.Get(levySchemeDeclaration.AccountId, levySchemeDeclaration.EmpRef, levySchemeDeclaration.PayrollYear,
                (byte)levySchemeDeclaration.PayrollMonth.Value);
            Logger.Debug("Now adding levy declaration to levy period.");
            levyDeclaration.RegisterLevyDeclaration(levySchemeDeclaration.LevyDeclaredInMonth, levySchemeDeclaration.CreatedDate);
            Logger.Debug($"Now storing the levy period. Employer: {levySchemeDeclaration.AccountId}, year: {levySchemeDeclaration.PayrollYear}, month: {levySchemeDeclaration.PayrollMonth}");
            await Repository.Store(levyDeclaration);
            Logger.Info($"Finished adding the levy declaration to the levy period. Levy declaration: {levyDeclaration.Id}");
            _queueService.SendMessageWithVisibilityDelay(levySchemeDeclaration, allowProjectionsEndpoint);

        }
    }
}