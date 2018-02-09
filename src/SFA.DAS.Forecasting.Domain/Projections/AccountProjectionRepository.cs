using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Accounts;
using SFA.DAS.Forecasting.Domain.Commitments;

namespace SFA.DAS.Forecasting.Domain.Projections
{
    public interface IAccountProjectionRepository
    {
        Task<AccountProjection> Get(long employerAccountId);
        Task Store(List<ReadModel.Projections.AccountProjection> accountProjections);
    }

    public class AccountProjectionRepository: IAccountProjectionRepository
    {
        private readonly IEmployerCommitmentsRepository _commitmentsRepository;
        private readonly IAccountRepository _accountRepository;

        public AccountProjectionRepository(IEmployerCommitmentsRepository commitmentsRepository, IAccountRepository accountRepository)
        {
            _commitmentsRepository = commitmentsRepository ?? throw new ArgumentNullException(nameof(commitmentsRepository));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        }

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