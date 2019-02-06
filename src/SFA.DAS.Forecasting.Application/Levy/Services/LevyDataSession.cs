using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Models.Levy;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Levy;

namespace SFA.DAS.Forecasting.Application.Levy.Services
{
    public class LevyDataSession : ILevyDataSession
    {
        private readonly IForecastingDataContext _dataContext;

        public LevyDataSession(IForecastingDataContext dataContext)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task<List<LevyDeclarationModel>> GetLevyDeclarationsForPeriod(long employerAccountId, string payrollYear, byte payrollMonth)
        {
            return await _dataContext.LevyDeclarations.Where(levy =>
                    levy.EmployerAccountId == employerAccountId &&
                    levy.PayrollYear == payrollYear &&
                    levy.PayrollMonth == payrollMonth)
                .ToListAsync();
        }

        public async Task<decimal> GetLatestLevyAmount(long employerAccountId)
        {
            return await _dataContext.LevyDeclarations
                .Where(levy => levy.EmployerAccountId == employerAccountId)
                .GroupBy(levy => levy.PayrollDate)
                    .Select(group => new
                    {
                        PayrollDate = group.Key,
                        Amount = group.Sum(levy => levy.LevyAmountDeclared)
                    })
                .OrderByDescending(item => item.PayrollDate)
                .Select(item => item.Amount)
                .FirstOrDefaultAsync();
        }


        public async Task<decimal> GetLatestPositiveLevyAmount(long employerAccountId)
        {
            return await _dataContext.LevyDeclarations
                .Where(levy => levy.EmployerAccountId == employerAccountId)
                .GroupBy(levy => levy.PayrollDate)
                .Select(group => new
                {
                    PayrollDate = group.Key,
                    Amount = group.Sum(levy => levy.LevyAmountDeclared)
                })
                .OrderByDescending(item => item.PayrollDate).Where(w => w.Amount > 0)
                .Select(item => item.Amount)
                .FirstOrDefaultAsync();
        }

        public async Task<LevyDeclarationModel> Get(long employerAccountId, string scheme, string payrollYear, byte payrollMonth)
        {
            return await _dataContext.LevyDeclarations.FirstOrDefaultAsync(levy =>
                levy.EmployerAccountId == employerAccountId && levy.Scheme == scheme &&
                levy.PayrollMonth == payrollMonth && levy.PayrollYear == payrollYear);
        }

        public async Task<LevyPeriod> GetNetTotals(long employerAccountId, string payrollYear, byte payrollMonth)
        {
            var netTotals = GetLevyDeclarationTotals(w =>
                w.EmployerAccountId == employerAccountId && w.PayrollYear == payrollYear &&
                w.PayrollMonth == payrollMonth).FirstOrDefault();

            return netTotals;
        }

        public async Task<IEnumerable<LevyPeriod>> GetAllNetTotals(long employerAccountId)
        {
            var netTotals = GetLevyDeclarationTotals(w => w.EmployerAccountId == employerAccountId);

            return netTotals;
        }

        private IEnumerable<LevyPeriod> GetLevyDeclarationTotals(Expression<Func<LevyDeclarationModel,bool>> wherePredicate)
        {
            return _dataContext.LevyDeclarations
                .Where(wherePredicate)
                .GroupBy(g => new { g.EmployerAccountId, g.TransactionDate.Year, g.TransactionDate.Month }).ToList()
                .Select(s => new LevyPeriod(s.Key.EmployerAccountId, s.Key.Year.ToString(), (byte)s.Key.Month,s.Max(v => v.PayrollDate), s.Sum(v => v.LevyAmountDeclared), s.Max(v => v.DateReceived)));
        }


        public async Task SaveChanges()
        {
            await _dataContext.SaveChangesAsync();
        }

        public void Store(LevyDeclarationModel levyDeclaration)
        {
            //TODO: assumes all entities were originally retreived from the data context if Id > 0
            if (levyDeclaration.Id <= 0)
                _dataContext.LevyDeclarations.Add(levyDeclaration);
        }
    }
}
