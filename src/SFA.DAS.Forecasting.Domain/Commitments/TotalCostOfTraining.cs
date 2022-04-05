using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public class CostOfTraining
    {
        public decimal LevyFunded { get; set; }
        public decimal TransferOut { get; internal set; }
        public IList<long> CommitmentIds { get; set; }
        public decimal TransferIn { get; set; }
        public decimal ApprovedPledgeApplicationCost { get; set; }
        public decimal AcceptedPledgeApplicationCost { get; set; }
        public decimal PledgeOriginatedCommitmentCost { get; set; }
    }
}