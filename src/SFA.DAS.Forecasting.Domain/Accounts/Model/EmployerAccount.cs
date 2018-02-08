using System;

namespace SFA.DAS.Forecasting.Domain.Accounts.Model
{
    public class EmployerAccount
    {
        public long EmployerAccountId { get; set; }
        public decimal LevyDeclared { get; set; }
        public DateTime LevyPeriod { get; set; }
        public decimal Balance { get; set; }
        public DateTime BalancePeriod { get; set; }
    }
}