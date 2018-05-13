using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Domain.Shared;

namespace SFA.DAS.Forecasting.Domain.Levy
{
    public interface ILevyPeriodRepository
    {
        Task<LevyPeriod> Get(long employerAccountId, string payrollYear, short payrollMonth);
    }

    public class LevyPeriodRepository : ILevyPeriodRepository
    {
        public ILevyDataSession LevyDataSession { get; }
        public IPayrollDateService PayrollDateService { get; }

        public LevyPeriodRepository(ILevyDataSession levyDataSession, IPayrollDateService payrollDateService)
        {
            LevyDataSession = levyDataSession ?? throw new ArgumentNullException(nameof(levyDataSession));
            PayrollDateService = payrollDateService ?? throw new ArgumentNullException(nameof(payrollDateService));
        }

        public async Task<LevyPeriod> Get(long employerAccountId, string payrollYear, short payrollMonth)
        {
            var levyDeclarations =
                await LevyDataSession.GetLevyDeclarationsForPeriod(employerAccountId, payrollYear, (byte)payrollMonth);
            var levyPeriod = new LevyPeriod(levyDeclarations);
            return levyPeriod;
        }
    }
}