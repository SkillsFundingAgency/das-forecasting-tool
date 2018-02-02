using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.Forecasting.Domain.Entities;

namespace SFA.DAS.Forecasting.Domain.Interfaces
{
    public interface IBalanceRepository
    {
        Task<EmployerBalance> GetEmployerBalanceAndLevyAsync(long employerId);

        Task<IEnumerable<BalanceItem>> GetBalanceAsync(long employerId);
    }
}
