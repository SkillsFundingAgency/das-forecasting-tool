using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Balance.Services;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Commitments.Services;

namespace SFA.DAS.Forecasting.Domain.Balance
{
    public interface ICurrentBalanceRepository
    {
        Task<CurrentBalance> Get(long employerAccountId);
        Task Store(CurrentBalance currentBalance);
    }

    public class CurrentBalanceRepository : ICurrentBalanceRepository
    {
        private readonly IBalanceDataService _dataService;
        private readonly IAccountBalanceService _accountBalanceService;
        private readonly IEmployerCommitmentsRepository _commitmentsRepository;

        public CurrentBalanceRepository(
            IBalanceDataService dataService, 
            IAccountBalanceService accountBalanceService,
            IEmployerCommitmentsRepository commitmentsRepository)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _accountBalanceService = accountBalanceService ?? throw new ArgumentNullException(nameof(accountBalanceService));
            _commitmentsRepository = commitmentsRepository ?? throw new ArgumentNullException(nameof(commitmentsRepository));
        }
        public async Task<CurrentBalance> Get(long employerAccountId)
        {
            var employerAccount = await _dataService.Get(employerAccountId) ?? new Models.Balance.BalanceModel { EmployerAccountId = employerAccountId, BalancePeriod = DateTime.MinValue };
            var employerCommitments = await _commitmentsRepository.Get(employerAccountId);
            return new CurrentBalance(employerAccount, _accountBalanceService, employerCommitments);
        }

        public async Task Store(CurrentBalance currentBalance)
        {
            await _dataService.Store(currentBalance.Model);
        }
    }
}