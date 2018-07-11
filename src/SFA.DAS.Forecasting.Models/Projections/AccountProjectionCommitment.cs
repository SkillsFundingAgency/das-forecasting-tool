using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Models.Projections
{
    public  class AccountProjectionCommitment
    {
        public long Id { get; set; } // Id (Primary key)
        public long AccountProjectionId { get; set; } // AccountProjectionId
        public long CommitmentId { get; set; } // CommitmentId

    }
}