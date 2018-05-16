using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public class CompletionPayments
    {
        public decimal LevyFundedCompletionPayment { get; set; }
        public decimal TransferInCompletionPayment { get; set; }
        public decimal TransferOutCompletionPayment { get; set; }
        public IList<long> CommitmentIds { get; set; }
    }
}