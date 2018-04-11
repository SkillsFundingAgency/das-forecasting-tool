using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Domain.Estimations.Services
{
    public interface IAccountEstimationDataService
    {
        Task<AccountEstimationModel> Get(long accountId);
        Task Store(AccountEstimationModel model);
    }

    public class AccountEstimationDataService : IAccountEstimationDataService
    {
        public Task<AccountEstimationModel> Get(long accountId)
        {
            throw new System.NotImplementedException();
        }

        public Task Store(AccountEstimationModel model)
        {
            throw new System.NotImplementedException();
        }
    }


}