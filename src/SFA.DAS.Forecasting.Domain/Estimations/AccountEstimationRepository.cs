using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Estimations.Services;

namespace SFA.DAS.Forecasting.Domain.Estimations
{
    
    public interface IAccountEstimationRepository
    {
        Task<AccountEstimation> Get(long accountId, string accountEstimationName = "default");
    }

    public class AccountEstimationRepository: IAccountEstimationRepository
    {
        private readonly IAccountEstimationDataService _dataService;

        public AccountEstimationRepository(IAccountEstimationDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }

        public async Task<AccountEstimation> Get(long accountId, string accountEstimationName = "default")
        {
            throw new System.NotImplementedException();
        }
    }
}