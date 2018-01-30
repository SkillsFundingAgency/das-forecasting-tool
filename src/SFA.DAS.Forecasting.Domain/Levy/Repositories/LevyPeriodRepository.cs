using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Levy.Aggregates;
using SFA.DAS.Forecasting.Domain.Levy.Services;

namespace SFA.DAS.Forecasting.Domain.Levy.Repositories
{
    public interface ILevyPeriodRepository
    {
        Task<LevyPeriod> Get(long employerAccountId, string periodYear, short periodMonth);
        Task StoreLevyPeriod(LevyPeriod levyPeriod);
    }

    public class LevyPeriodRepository: ILevyPeriodRepository
    {
        public ILevyDataService LevyDataService { get; }

        public LevyPeriodRepository(ILevyDataService levyDataService)
        {
            LevyDataService = levyDataService ?? throw new ArgumentNullException(nameof(levyDataService));
        }

        public async Task<LevyPeriod> Get(long employerAccountId, string periodYear, short periodMonth)
        {
            var levyDeclarations =
                await LevyDataService.GetLevyDeclarationsForPeriod(employerAccountId, periodYear, periodMonth);
            var levyPeriod = new LevyPeriod();
            levyPeriod.LevyDeclarations.AddRange(levyDeclarations);
            return levyPeriod;
        }

        public async Task StoreLevyPeriod(LevyPeriod levyPeriod)
        {
            foreach (var levyDeclaration in levyPeriod.LevyDeclarations)
            {
                await LevyDataService.StoreLevyDeclaration(levyDeclaration);
            }
        }
    }
}