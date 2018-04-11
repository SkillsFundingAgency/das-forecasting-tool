using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Estimations.Services;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Estimations.Services
{
    public class AccountEstimationDataService: IAccountEstimationDataService
    {
        public Task<AccountEstimationModel> Get(long accountId)
        {
            return null;
        }

        public Task Store(AccountEstimationModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}