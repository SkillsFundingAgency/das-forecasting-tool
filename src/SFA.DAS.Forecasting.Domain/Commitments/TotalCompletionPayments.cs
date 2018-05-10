using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public class CompletionPayments
    {
        public decimal LevyCompletionPaymentOut { get; set; }
        public decimal TransferCompletionPaymentOut { get; internal set; }
        public IEnumerable<long> CommitmentIds { get; set; }
        public decimal TransferCompletionPaymentIn { get; set; }
    }
}