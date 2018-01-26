using SFA.DAS.Forecasting.Levy.Domain.Repositories;
using SFA.DAS.NLog.Logger;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Levy.Domain.Aggregates
{
    public class EmployerLevy
    {
        private readonly IEmployerLevyRepository _levyStorage;
        private readonly ILog _logger;

        public EmployerLevy(IEmployerLevyRepository levyStorage, ILog logger)
        {
            _levyStorage = levyStorage;
            _logger = logger;
        }

        public async Task AddDeclaration(long employerAccountId, DateTime payrollDate, decimal amount, string scheme, DateTime transactionDate)
        {
            if (transactionDate < DateTime.Now.AddMonths(-25))
            {
                _logger.Info("Found LevyDeclarationEvent older than 2 years. Not saved.");
                return;
            }

            var levyDeclaration = new Entities.LevyDeclaration
            {
                EmployerAccountId = employerAccountId,
                Amount = amount,
                Scheme = scheme,
                Period = $"{payrollDate.Year}-{payrollDate.Month}",
                TransactionDate = transactionDate
            };
            
            await _levyStorage.StoreLevyDeclaration(levyDeclaration);
        }
    }
}