using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public class CostOfTraining
    {
        public decimal LevyOut { get; set; }
        public decimal TransferOut { get; internal set; }
        public IEnumerable<long> CommitmentIds { get; set; }
        public decimal TransferIn { get; set; }
    }
}