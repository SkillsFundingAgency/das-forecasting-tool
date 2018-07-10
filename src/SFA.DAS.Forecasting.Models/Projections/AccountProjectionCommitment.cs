using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Models.Projections
{
    public  class AccountProjectionCommitment
    {
        public long Id { get; set; } // Id (Primary key)
        public long AccountProjectionId { get; set; } // AccountProjectionId
        public long CommitmentId { get; set; } // CommitmentId

        // Foreign keys

        /// <summary>
        /// Parent AccountProjection pointed by [AccountProjectionCommitment].([AccountProjectionId]) (FK_AccountProjectionCommitment__AccountProjection)
        /// </summary>
        public virtual AccountProjectionMonth AccountProjection { get; set; } // FK_AccountProjectionCommitment__AccountProjection

        /// <summary>
        /// Parent Commitment pointed by [AccountProjectionCommitment].([CommitmentId]) (FK_AccountProjectionCommitment__Commitment)
        /// </summary>
        public virtual CommitmentModel Commitment { get; set; } // FK_AccountProjectionCommitment__Commitment

        public AccountProjectionCommitment()
        {

        }
    }
}