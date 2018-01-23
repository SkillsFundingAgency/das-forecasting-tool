using SFA.DAS.Forecasting.Levy.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Levy.Domain.Aggregates
{
    public class EmployerLevy
    {
        private readonly IEmployerLevyRepository _levyStorage;

        public EmployerLevy(IEmployerLevyRepository levyStorage)
        {
            _levyStorage = levyStorage;
        }
        public async Task AddDeclaration(long employerAccountId, DateTime payrollDate, decimal amount, string scheme, DateTime transactionDate)
        {
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

        public async Task<decimal> GetCurrentPeriodTotalAmount(string employerAccountId)
        {
            throw new NotImplementedException();
        }
    }
}