using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Estimations.Services;
using SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Domain.Estimations
{

    public interface IAccountEstimationRepository
    {
        Task<AccountEstimation> Get(long accountId);
        Task Store(AccountEstimation accountEstimation);
    }

    public class AccountEstimationRepository : IAccountEstimationRepository
    {
        private readonly IAccountEstimationDataService _dataService;
        private readonly IVirtualApprenticeshipValidator _validator;

        public AccountEstimationRepository(IAccountEstimationDataService dataService, IVirtualApprenticeshipValidator validator)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task<AccountEstimation> Get(long accountId)
        {
            var model = await _dataService.Get(accountId) ?? new AccountEstimationModel
            {
                Id = Guid.NewGuid().ToString("D"),
                EstimationName = "default",
                EmployerAccountId = accountId
            };
            return new AccountEstimation(model, _validator);
        }

        public async Task Store(AccountEstimation accountEstimation)
        {
            await _dataService.Store(accountEstimation.Model);
        }
    }
}