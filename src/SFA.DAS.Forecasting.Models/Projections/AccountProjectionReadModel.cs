using System;
using System.Collections.Generic;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Models.Projections
{
    //public class AccountProjectionReadModel
    //{
    //    public long Id { get; set; }
    //    public long EmployerAccountId { get; set; }
    //    public DateTime ProjectionCreationDate { get; set; }
    //    public ProjectionGenerationType ProjectionGenerationType { get; set; }
    //    public short Month { get; set; }
    //    public int Year { get;set; }
    //    public decimal FundsIn { get; set; }
    //    public decimal TotalCostOfTraining { get; set; }
    //    public decimal CompletionPayments { get; set; }
    //    public decimal FutureFunds { get; set; }
    //    public List<long> Commitments { get; set; } = new List<long>();
    //    public decimal CoInvestmentEmployer { get; set; }
    //    public decimal CoInvestmentGovernment { get; set; }
    //}

    public class AccountProjectionReadModel
    {
        public long Id { get; set; } // Id (Primary key)
        public long EmployerAccountId { get; set; } // EmployerAccountId
        public System.DateTime ProjectionCreationDate { get; set; } // ProjectionCreationDate
        public short ProjectionGenerationType { get; set; } // ProjectionGenerationType
        public short Month { get; set; } // Month
        public int Year { get; set; } // Year
        public decimal FundsIn { get; set; } // FundsIn
        public decimal TotalCostOfTraining { get; set; } // TotalCostOfTraining
        public decimal CompletionPayments { get; set; } // CompletionPayments
        public decimal FutureFunds { get; set; } // FutureFunds
        public decimal CoInvestmentEmployer { get; set; } // CoInvestmentEmployer
        public decimal CoInvestmentGovernment { get; set; } // CoInvestmentGovernment

        // Reverse navigation

        /// <summary>
        /// Child AccountProjectionCommitments where [AccountProjectionCommitment].[AccountProjectionId] point to this entity (FK_AccountProjectionCommitment__AccountProjection)
        /// </summary>
        public virtual System.Collections.Generic.ICollection<AccountProjectionCommitment> Commitments { get; set; } // AccountProjectionCommitment.FK_AccountProjectionCommitment__AccountProjection

        public AccountProjectionReadModel()
        {
            CoInvestmentEmployer = 0m;
            CoInvestmentGovernment = 0m;
            Commitments = new System.Collections.Generic.List<AccountProjectionCommitment>();
        }
    }

    public  class AccountProjectionCommitment
    {
        public long Id { get; set; } // Id (Primary key)
        public long AccountProjectionId { get; set; } // AccountProjectionId
        public long CommitmentId { get; set; } // CommitmentId

        // Foreign keys

        /// <summary>
        /// Parent AccountProjection pointed by [AccountProjectionCommitment].([AccountProjectionId]) (FK_AccountProjectionCommitment__AccountProjection)
        /// </summary>
        public virtual AccountProjectionReadModel AccountProjectionReadModel { get; set; } // FK_AccountProjectionCommitment__AccountProjection

        /// <summary>
        /// Parent Commitment pointed by [AccountProjectionCommitment].([CommitmentId]) (FK_AccountProjectionCommitment__Commitment)
        /// </summary>
        public virtual CommitmentModel Commitment { get; set; } // FK_AccountProjectionCommitment__Commitment

        public AccountProjectionCommitment()
        {

        }
    }
}