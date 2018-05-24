using System;

namespace SFA.DAS.Forecasting.AcceptanceTests.Payments
{
    public class TestAccountProjection
    {
        public int MonthsFromNow { get; set; }

        public decimal TotalCostOfTraining { get; set; }
        public decimal TransferOutTotalCostOfTraining { get; set; }
        public decimal TransferInTotalCostOfTraining { get; set; }

        public decimal TransferOutCompletionPayments { get; set; }
        public decimal TransferInCompletionPayments { get; set; }
        public decimal CompletionPayments { get; set; }

        public decimal FutureFunds { get; set; }
    }
}
