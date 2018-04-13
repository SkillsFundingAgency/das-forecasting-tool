using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Domain.Estimations.Services
{
    public interface IAccountEstimationDataService
    {
        Task<AccountEstimationModel> Get(long accountId);
        Task Store(AccountEstimationModel model);
    }
}