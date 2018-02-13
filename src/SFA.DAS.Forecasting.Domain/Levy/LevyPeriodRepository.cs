using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Domain.Shared;

namespace SFA.DAS.Forecasting.Domain.Levy
{
    public interface ILevyPeriodRepository
    {
        Task<LevyPeriod> Get(long employerAccountId, string payrollYear, short payrollMonth);
        Task StoreLevyPeriod(LevyPeriod levyPeriod);
    }

    public class LevyPeriodRepository: ILevyPeriodRepository
    {
        public ILevyDataService LevyDataService { get; }
        public IPayrollDateService PayrollDateService { get; }

        public LevyPeriodRepository(ILevyDataService levyDataService, IPayrollDateService payrollDateService)
        {
            LevyDataService = levyDataService ?? throw new ArgumentNullException(nameof(levyDataService));
            PayrollDateService = payrollDateService ?? throw new ArgumentNullException(nameof(payrollDateService));
        }

        public async Task<LevyPeriod> Get(long employerAccountId, string payrollYear, short payrollMonth)
        {
            var levyDeclarations =
                await LevyDataService.GetLevyDeclarationsForPeriod(employerAccountId, payrollYear, (byte)payrollMonth);
            var levyPeriod = new LevyPeriod(PayrollDateService);
            levyPeriod.LevyDeclarations.AddRange(levyDeclarations);
            return levyPeriod;
        }

        public async Task StoreLevyPeriod(LevyPeriod levyPeriod)
        {
            await LevyDataService.StoreLevyDeclarations(levyPeriod.LevyDeclarations);
        }
    }
}