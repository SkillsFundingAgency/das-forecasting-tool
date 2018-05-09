using System;
using System.Collections.Generic;

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

    public class AccountProjectionModel
    {
        public long Id { get; set; } // Id (Primary key)
        public long EmployerAccountId { get; set; } // EmployerAccountId
        public System.DateTime ProjectionCreationDate { get; set; } // ProjectionCreationDate
        public ProjectionGenerationType ProjectionGenerationType { get; set; } // ProjectionGenerationType
        public short Month { get; set; } // Month
        public int Year { get; set; } // Year
        public decimal FundsIn { get; set; } // FundsIn
        public decimal TotalCostOfTraining { get; set; } // TotalCostOfTraining
        public decimal TransferInTotalCostOfTraining { get; set; }
        public decimal TransferOutTotalCostOfTraining { get; set; }
        public decimal CompletionPayments { get; set; } // CompletionPayments
        public decimal TransferInCompletionPayments { get; set; }
        public decimal TransferOutCompletionPayments { get; set; }
        public decimal FutureFunds { get; set; } // FutureFunds
        public decimal CoInvestmentEmployer { get; set; } // CoInvestmentEmployer
        public decimal CoInvestmentGovernment { get; set; } // CoInvestmentGovernment

        // Reverse navigation

        /// <summary>
        /// Child AccountProjectionCommitments where [AccountProjectionCommitment].[AccountProjectionId] point to this entity (FK_AccountProjectionCommitment__AccountProjection)
        /// </summary>
        public virtual System.Collections.Generic.ICollection<AccountProjectionCommitment> Commitments { get; set; } // AccountProjectionCommitment.FK_AccountProjectionCommitment__AccountProjection

        public AccountProjectionModel()
        {
            CoInvestmentEmployer = 0m;
            CoInvestmentGovernment = 0m;
            Commitments = new List<AccountProjectionCommitment>();
        }

    }
}