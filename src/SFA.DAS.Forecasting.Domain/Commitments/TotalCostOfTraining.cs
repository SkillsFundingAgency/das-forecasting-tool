using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Domain.Commitments
{
    public class TotalCostOfTraining
    {
        public decimal Value { get; set; }
        public IEnumerable<long> CommitmentIds { get; set; }
        public decimal TransferCost { get; set; }
    }
}