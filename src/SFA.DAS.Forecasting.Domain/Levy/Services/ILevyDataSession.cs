using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Levy;

namespace SFA.DAS.Forecasting.Domain.Levy.Services
{
    public interface ILevyDataSession
    {
        Task<List<LevyDeclarationModel>> GetLevyDeclarationsForPeriod(long employerAccountId, string payrollYear, byte payrollMonth);
        Task<decimal> GetLatestLevyAmount(long employerAccountId);
        Task<LevyDeclarationModel> Get(long employerAccountId, string scheme, string payrollYear, byte payrollMonth);
        void Store(LevyDeclarationModel model);
        Task SaveChanges();
    }
}