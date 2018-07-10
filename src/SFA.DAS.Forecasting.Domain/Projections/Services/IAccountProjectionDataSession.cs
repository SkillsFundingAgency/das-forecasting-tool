using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.Projections.Services
{
    public interface IAccountProjectionDataSession
    {
        Task<List<AccountProjectionModel>> Get(long employerId);
        Task Store(IEnumerable<AccountProjectionModel> accountProjections);
        Task DeleteAll(long employerAccountId);
        Task SaveChanges();
    }
}
