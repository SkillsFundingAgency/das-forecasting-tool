using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Levy.Model;

namespace SFA.DAS.Forecasting.Domain.Levy.Services
{
    public interface ILevyDataService
    {
        Task<List<LevyDeclaration>> GetLevyDeclarationsForPeriod(long employerAccountId, string periodYear, int month);
        Task StoreLevyDeclaration(LevyDeclaration levyDeclaration);
    }
}