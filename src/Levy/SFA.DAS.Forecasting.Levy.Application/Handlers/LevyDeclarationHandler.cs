using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Levy.Application.Messages;
using SFA.DAS.Forecasting.Levy.Domain.Repositories;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Levy.Application.Handlers
{
    public class LevyDeclarationHandler
    {
        public ILevyPeriodRepository Repository { get; }
        public ILog Logger { get; }

        public LevyDeclarationHandler(ILevyPeriodRepository repository, ILog logger)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(LevyDeclarationEvent levyDeclaration)
        {
            Logger.Debug($"Now storing the levy declaration event: {JsonConvert.SerializeObject(levyDeclaration)}");
            var levyPeriod = await Repository.Get(levyDeclaration.EmployerAccountId, levyDeclaration.PayrollYear,
                levyDeclaration.PayrollMonth ?? 0);
            Logger.Debug("Now adding levy declaration to levy period.");
            levyPeriod.AddDeclaration(levyDeclaration.EmployerAccountId, levyDeclaration.PayrollYear, levyDeclaration.PayrollMonth ?? 0, levyDeclaration.Amount, levyDeclaration.Scheme, levyDeclaration.TransactionDate);
            Logger.Debug($"Now storing the levy period. Employer: {levyDeclaration.EmployerAccountId}, year: {levyDeclaration.PayrollYear}, month: {levyDeclaration.PayrollMonth}");
            await Repository.StoreLevyPeriod(levyPeriod);
            Logger.Info($"Finished adding the levy declaration to the levy period. Levy declaration: {JsonConvert.SerializeObject(levyDeclaration)}");
        }
    }
}