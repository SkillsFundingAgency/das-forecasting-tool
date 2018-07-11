using System;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Models.Projections
{
    public class AccountProjectionModel
    {
        public long Id { get; set; } // Id (Primary key)
        public long EmployerAccountId { get; set; } // EmployerAccountId
        public System.DateTime ProjectionCreationDate { get; set; } // ProjectionCreationDate
        public ProjectionGenerationType ProjectionGenerationType { get; set; } // ProjectionGenerationType
        public short Month { get; set; } // Month
        public int Year { get; set; } // Year

        public decimal LevyFundsIn { get; set; }
        public decimal LevyFundedCostOfTraining { get; set; }
        public decimal LevyFundedCompletionPayments { get; set; }


        public decimal TransferInCostOfTraining { get; set; }
        public decimal TransferOutCostOfTraining { get; set; }

        public decimal TransferInCompletionPayments { get; set; }
        public decimal TransferOutCompletionPayments { get; set; }

        public decimal CommittedTransferCost { get; set; }
        public decimal CommittedTransferCompletionCost { get; set; }
        public decimal FutureFunds { get; set; } // FutureFunds
        public decimal CoInvestmentEmployer { get; set; } // CoInvestmentEmployer
        public decimal CoInvestmentGovernment { get; set; } // CoInvestmentGovernment

        
        public AccountProjectionModel()
        {
            CoInvestmentEmployer = 0m;
            CoInvestmentGovernment = 0m;
        }

    }
}