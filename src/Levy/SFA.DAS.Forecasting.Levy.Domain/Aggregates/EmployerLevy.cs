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
        public async Task AddDeclaration(string employerAccountId, string period, decimal amount, string scheme, DateTime transactionDate)
        {   
            _levyStorage.StoreLevyDeclaration( // await?
                new Entities.LevyDeclaration
                {
                    EmployerAccountId = employerAccountId,
                    Amount = amount,
                    Scheme = scheme,
                    Period = "todo - add period", // ToDo: Added period 
                    TransactionDate = transactionDate
                });
            await Task.FromResult(1); //remove
        }

        public async Task<decimal> GetCurrentPeriodTotalAmount(string employerAccountId)
        {
            throw new NotImplementedException();
        }
    }
}