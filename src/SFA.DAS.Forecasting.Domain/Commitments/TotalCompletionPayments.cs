using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public class TotalCompletionPayments
    {
        public decimal LevyCompletionPaymentReceived { get; set; }
        public IEnumerable<long> CommitmentIds { get; set; }
        public decimal TransferOut { get; set; }
        public decimal TransferCompletionPayment { get; internal set; }
    }
}