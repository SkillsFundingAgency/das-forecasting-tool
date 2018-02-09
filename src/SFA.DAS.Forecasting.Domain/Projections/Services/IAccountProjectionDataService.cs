using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Domain.Projections.Services
{
    public interface IAccountProjectionDataService
    {
        Task<IEnumerable<ReadModel.Projections.AccountProjection>> Get(long employerId);
        Task Refresh(long employerAccountId, IEnumerable<AccountProjection> accountProjections);
    }
}
