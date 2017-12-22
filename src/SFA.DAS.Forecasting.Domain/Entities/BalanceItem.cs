using System;

namespace SFA.DAS.Forecasting.Domain.Entities
{
    public class BalanceItem
    {
        public DateTime Date { get; set; }

        public int LevyCredit { get; set; }

        public int CostOfTraining { get; set; }

        public int CompletionPayments { get; set; }

        public int ExpiredFunds { get; set; }

        public int Balance { get; set; }
    }

    public class EmployerBalance
    {
        public long EmployerAccountId { get; set; }

        public decimal LevyCredit { get; set; }

        public decimal Balance { get; set; }
    }
}
