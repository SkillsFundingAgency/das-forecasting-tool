namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public class CostOfPledges
    {
        public decimal ApprovedPledgeApplicationCost { get; set; }
        public decimal AcceptedPledgeApplicationCost { get; set; }
        public decimal PledgeOriginatedCommitmentCost { get; set; }
        public decimal ApprovedPledgeApplicationCompletionPayments { get; set; }
        public decimal AcceptedPledgeApplicationCompletionPayments { get; set; }
        public decimal PledgeOriginatedCommitmentCompletionPayments { get; set; }
    }
}
