namespace SFA.DAS.Forecasting.Models.Estimation
{
    public class AccountEstimationProjectionModel
    {
        public short Month { get; set; }
        public short Year { get; set; }
        public decimal TotalCostOfTraining { get; set; }
        public decimal TransferInTotalCostOfTraining { get; set; }
        public decimal TransferOutTotalCostOfTraining { get; set; }
        public decimal CompletionPayments { get; set; }
        public decimal TransferOutCompletionPayments { get; set; }
        public decimal ActualCommittedTransferCost { get; set; }
        public decimal ActualCommittedTransferCompletionCost { get; set; }
        public decimal FutureFunds { get; set; }
        public decimal TransferInCompletionPayments { get; set; }
    }
}
