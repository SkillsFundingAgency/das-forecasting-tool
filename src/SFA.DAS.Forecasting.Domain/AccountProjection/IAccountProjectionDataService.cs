using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Domain.AccountProjection
{
    public interface IAccountProjectionDataService
    {
        Task<IEnumerable<ReadModel.AccountProjections.AccountProjection>> Get(long employerId);
    }
}
