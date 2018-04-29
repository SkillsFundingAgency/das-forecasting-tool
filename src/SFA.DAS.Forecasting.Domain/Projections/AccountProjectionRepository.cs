using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Models.Balance;
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
        private readonly ILevyDataSession _levyDataSession;
        private readonly IAccountProjectionDataSession _accountProjectionDataSession;

        public AccountProjectionRepository(IEmployerCommitmentsRepository commitmentsRepository, ICurrentBalanceRepository currentBalanceRepository,
            ILevyDataSession levyDataSession, IAccountProjectionDataSession accountProjectionDataSession)
        {
            _commitmentsRepository = commitmentsRepository ?? throw new ArgumentNullException(nameof(commitmentsRepository));
            _currentBalanceRepository = currentBalanceRepository ?? throw new ArgumentNullException(nameof(currentBalanceRepository));
            _levyDataSession = levyDataSession ?? throw new ArgumentNullException(nameof(levyDataSession));
            _accountProjectionDataSession = accountProjectionDataSession ?? throw new ArgumentNullException(nameof(accountProjectionDataSession));
        }

        public async Task<AccountProjection> Get(long employerAccountId)
        {
            var levy = await _levyDataSession.GetLatestLevyAmount(employerAccountId);
            var balance = await _currentBalanceRepository.Get(employerAccountId);
            var commitments = await _commitmentsRepository.Get(employerAccountId);
            
            return new AccountProjection(new Account(employerAccountId, balance.Amount, levy, balance.TransferAllowance, balance.TransferAllowance), commitments);
        }

        public async Task Store(AccountProjection accountProjection)
        {
            if (!accountProjection.Projections.Any())
                return;
            await _accountProjectionDataSession.DeleteAll(accountProjection.EmployerAccountId);
            _accountProjectionDataSession.Store(accountProjection.Projections);
            await _accountProjectionDataSession.SaveChanges();
        }
    }
}