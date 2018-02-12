using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.Projections
{
    public interface IAccountProjectionRepository
    {
        Task<AccountProjection> Get(long employerAccountId);
        Task Store(AccountProjection accountProjection);
    }

    public class AccountProjectionRepository : IAccountProjectionRepository
    {
        private readonly IEmployerCommitmentsRepository _commitmentsRepository;
        private readonly ICurrentBalanceRepository _currentBalanceRepository;
        private readonly ILevyDataService _levyDataService;
        private readonly IAccountProjectionDataService _accountProjectionDataService;

        public AccountProjectionRepository(IEmployerCommitmentsRepository commitmentsRepository, ICurrentBalanceRepository currentBalanceRepository,
            ILevyDataService levyDataService, IAccountProjectionDataService accountProjectionDataService)
        {
            _commitmentsRepository = commitmentsRepository ?? throw new ArgumentNullException(nameof(commitmentsRepository));
            _currentBalanceRepository = currentBalanceRepository ?? throw new ArgumentNullException(nameof(currentBalanceRepository));
            _levyDataService = levyDataService ?? throw new ArgumentNullException(nameof(levyDataService));
            _accountProjectionDataService = accountProjectionDataService ?? throw new ArgumentNullException(nameof(accountProjectionDataService));
        }

        public async Task<AccountProjection> Get(long employerAccountId)
        {
            var levy = _levyDataService.GetLatestLevyAmount(employerAccountId);
            var balance = _currentBalanceRepository.Get(employerAccountId);
            var commitments = _commitmentsRepository.Get(employerAccountId);
            await Task.WhenAll(levy, balance, commitments);
            return new AccountProjection(new Account(employerAccountId, balance.Result.Amount, levy.Result), commitments.Result);
        }

        public async Task Store(AccountProjection accountProjection)
        {
            if (!accountProjection.Projections.Any())
                return;
            await _accountProjectionDataService.Store(accountProjection.Projections.First().EmployerAccountId, accountProjection.Projections);
        }
    }
}