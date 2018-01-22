using System;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Levy.Domain.Aggregates
{
    public class EmployerLevy
    {
        public EmployerLevy()
        {

        }
        public async Task AddDeclaration(string employerAccountId, string period, decimal amount, string scheme, DateTime transactionDate)
        {
            throw new NotImplementedException();
        }

        public async Task<decimal> GetCurrentPeriodTotalAmount(string employerAccountId)
        {
            throw new NotImplementedException();
        }
    }
}