using System;

namespace SFA.DAS.Forecasting.Models.Balance
{
    public class BalanceModel
    {
        public long EmployerAccountId { get; set; }
        public decimal Amount { get; set; }
        public decimal TransferAllowance { get; set; }
        public decimal RemainingTransferBalance { get; set; }
        public DateTime BalancePeriod { get; set; }
        public DateTime ReceivedDate { get; set; }
        public decimal UnallocatedCompletionPayments { get; set; }
    }
}