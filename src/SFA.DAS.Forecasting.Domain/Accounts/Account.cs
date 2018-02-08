using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Accounts.Model;
using SFA.DAS.Forecasting.Domain.Accounts.Services;

namespace SFA.DAS.Forecasting.Domain.Accounts
{
    public class Account
    {
        public long EmployerAccountId => _employerAccount.EmployerAccountId;
        public decimal LevyDeclared => _employerAccount.LevyDeclared;
        public DateTime LevyPeriod => _employerAccount.LevyPeriod;
        public decimal Balance => _employerAccount.Balance;
        public DateTime BalancePeriod => _employerAccount.BalancePeriod;
        internal readonly EmployerAccount _employerAccount;

        public Account(EmployerAccount employerAccount)
        {
            _employerAccount = employerAccount ?? throw new ArgumentNullException(nameof(employerAccount));
        }

        public void SetBalance(decimal balance, DateTime balancePeriod)
        {

        }

        public void SetLevy(decimal levy, DateTime levyPeriod)
        {

        }
    }


}