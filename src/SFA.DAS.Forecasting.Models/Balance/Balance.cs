using System;

namespace SFA.DAS.Forecasting.Models.Balance
{
    public class Balance
    {
        public long EmployerAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime BalancePeriod { get; set; }
        public DateTime ReceivedDate { get; set; }
    }
}