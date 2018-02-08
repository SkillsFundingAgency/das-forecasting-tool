using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Domain.Projections
{
    public interface IAccountProjectionRepository
    {
        Task<AccountProjection> Get(long employerAccountId);
        Task Store(List<ReadModel.Projections.AccountProjection> accountProjections);
    }

    public class AccountProjectionRepository: IAccountProjectionRepository
    {
        public Task<AccountProjection> Get(long employerAccountId)
        {
            throw new System.NotImplementedException();
        }

        public Task Store(List<ReadModel.Projections.AccountProjection> accountProjections)
        {
            throw new System.NotImplementedException();
        }
    }
}