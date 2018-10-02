using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Domain.Projections
{
    public interface IAccountProjectionRepository
    {
        Task<IList<AccountProjectionModel>> Get(long employerAccountId);
        Task<AccountProjection> InitialiseProjection(long employerAccountId);
        Task Store(AccountProjection accountProjection);
    }

    public class AccountProjectionRepository : IAccountProjectionRepository
    {
        private readonly ICurrentBalanceRepository _currentBalanceRepository;
        private readonly ILevyDataSession _levyDataSession;
        private readonly IAccountProjectionDataSession _accountProjectionDataSession;
 

        public AccountProjectionRepository(ICurrentBalanceRepository currentBalanceRepository,
            ILevyDataSession levyDataSession, IAccountProjectionDataSession accountProjectionDataSession)
        {
            _currentBalanceRepository = currentBalanceRepository ?? throw new ArgumentNullException(nameof(currentBalanceRepository));
            _levyDataSession = levyDataSession ?? throw new ArgumentNullException(nameof(levyDataSession));
            _accountProjectionDataSession = accountProjectionDataSession ?? throw new ArgumentNullException(nameof(accountProjectionDataSession));
        }

        public async Task<IList<AccountProjectionModel>> Get(long employerAccountId)
        {
            var projections = await _accountProjectionDataSession.Get(employerAccountId);

            if (!projections.Any()) return null;

            var firstProjection = projections.OrderBy(o => new { o.Year, o.Month }).First();
            firstProjection.IsFirstMonth = true;

            return projections;
        }

        public async Task<AccountProjection> InitialiseProjection(long employerAccountId)
        {
            var levy = await _levyDataSession.GetLatestLevyAmount(employerAccountId);

            if (levy < 0)
            {
                levy = await _levyDataSession.GetLatestPositiveLevyAmount(employerAccountId);
            }

            var balance = await _currentBalanceRepository.Get(employerAccountId);

            return new AccountProjection(new Account(employerAccountId, balance.Amount, levy, balance.TransferAllowance, balance.TransferAllowance), balance.EmployerCommitments);
        }

        public async Task Store(AccountProjection accountProjection)
        {
            if (!accountProjection.Projections.Any())
                return;
            await _accountProjectionDataSession.DeleteAll(accountProjection.EmployerAccountId);
            await _accountProjectionDataSession.Store(accountProjection.Projections);
            await _accountProjectionDataSession.SaveChanges();
        }
    }
}