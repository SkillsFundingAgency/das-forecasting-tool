using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Levy.Repositories;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Levy.Handlers
{
    public class ProcessLevyDeclarationHandler
    {
        public ILevyPeriodRepository Repository { get; }
        public ILog Logger { get; }

        public ProcessLevyDeclarationHandler(ILevyPeriodRepository repository, ILog logger)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(LevySchemeDeclarationUpdatedMessage levySchemeDeclaration)
        {
            Logger.Debug($"Now handling the levy declaration event: {levySchemeDeclaration.ToDebugJson()}");
            if (levySchemeDeclaration.PayrollMonth==null)
                throw new InvalidOperationException($"Received invalid levy declaration. No month specified. Data: ");
            var levyPeriod = await Repository.Get(levySchemeDeclaration.AccountId, levySchemeDeclaration.PayrollYear,
                levySchemeDeclaration.PayrollMonth.Value);
            Logger.Debug("Now adding levy declaration to levy period.");
            levyPeriod.AddDeclaration(levySchemeDeclaration.AccountId, levySchemeDeclaration.PayrollYear, (byte)levySchemeDeclaration.PayrollMonth.Value, 
                levySchemeDeclaration.LevyDeclaredInMonth, levySchemeDeclaration.EmpRef, levySchemeDeclaration.CreatedDate);
            Logger.Debug($"Now storing the levy period. Employer: {levySchemeDeclaration.AccountId}, year: {levySchemeDeclaration.PayrollYear}, month: {levySchemeDeclaration.PayrollMonth}");
            await Repository.StoreLevyPeriod(levyPeriod);
            Logger.Info($"Finished adding the levy declaration to the levy period. Levy declaration: {JsonConvert.SerializeObject(levySchemeDeclaration)}");
        }
    }
}