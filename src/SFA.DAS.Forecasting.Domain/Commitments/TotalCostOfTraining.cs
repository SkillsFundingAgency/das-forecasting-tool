using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public class CostOfTraining
    {
        public decimal LevyFunded { get; set; }
        public decimal TransferOut { get; internal set; }
        public IList<long> CommitmentIds { get; set; }
        public decimal TransferIn { get; set; }
    }
}