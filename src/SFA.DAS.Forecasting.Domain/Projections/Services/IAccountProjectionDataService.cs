using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Domain.Projections.Services
{
    public interface IAccountProjectionDataService
    {
        //Task<IEnumerable<ReadModel.Projections.AccountProjectionReadModel>> Get(long employerId);
        Task Store(long employerAccountId, IEnumerable<ReadModel.Projections.AccountProjectionReadModel> accountProjections);
    }
}
