namespace SFA.DAS.Forecasting.Models.Projections
{
    public class AccountProjectionMonth
    {
        public System.DateTime ProjectionCreationDate { get; set; }
        public ProjectionGenerationType ProjectionGenerationType { get; set; }
        public short Month { get; set; }
        public int Year { get; set; }

        public decimal LevyFundsIn { get; set; }
        public decimal LevyFundedCostOfTraining { get; set; }
        public decimal LevyFundedCompletionPayments { get; set; }


        public decimal TransferInCostOfTraining { get; set; }
        public decimal TransferOutCostOfTraining { get; set; }

        public decimal TransferInCompletionPayments { get; set; }
        public decimal TransferOutCompletionPayments { get; set; }

        public decimal CommittedTransferCost { get; set; }
        public decimal CommittedTransferCompletionCost { get; set; }
        public decimal FutureFunds { get; set; }
        public decimal CoInvestmentEmployer { get; set; } = 0M;
        public decimal CoInvestmentGovernment { get; set; } = 0M;
    }
}