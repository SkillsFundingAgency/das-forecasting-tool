using System;

namespace SFA.DAS.Forecasting.Domain.Projections.Model
{
    public class AccountActivity
    {
        public long EmployerAccountId { get; set; }
        public decimal LevyDeclared { get; set; }
        public DateTime? LevyPeriod { get; set; }
        public decimal Balance { get; set; }
        public DateTime BalanceDate { get; set; }
    }
}