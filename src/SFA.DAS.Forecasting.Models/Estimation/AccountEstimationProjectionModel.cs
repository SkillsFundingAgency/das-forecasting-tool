using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Models.Estimation
{
    public class AccountEstimationProjectionModel
    {
        public class Cost
        {
            public decimal LevyCostOfTraining { get; set; }
            public decimal LevyCompletionPayments { get; set; }
            public decimal TransferInCostOfTraining { get; set; }
            public decimal TransferInCompletionPayments { get; set; }
            public decimal TransferOutCostOfTraining { get; set; }
            public decimal TransferOutCompletionPayments { get; set; }
            public decimal TransferFundsIn => TransferInCostOfTraining + TransferInCompletionPayments;
            public decimal TransferFundsOut => TransferOutCostOfTraining + TransferOutCompletionPayments;
            public decimal ExpiredFunds { get; set; }
            public decimal FundsOut => LevyCostOfTraining + LevyCompletionPayments + TransferOutCostOfTraining +
                                           TransferOutCompletionPayments - TransferInCostOfTraining - TransferInCompletionPayments;
        }

        public short Month { get; set; }
        public short Year { get; set; }

        public Cost TransferModelledCosts { get; set; }
        public Cost ActualCosts { get; set; }
        public decimal TransferFundsIn => ActualCosts.TransferFundsIn + TransferModelledCosts.TransferFundsIn;
        public decimal TransferFundsOut => ActualCosts.TransferFundsOut + TransferModelledCosts.FundsOut;
        public AccountEstimationProjectionModel()
        {
            TransferModelledCosts = new Cost();
            ActualCosts = new Cost();
            AllModelledCosts = new Cost();
        }
        public ProjectionGenerationType ProjectionGenerationType { get; set; }
        public decimal AvailableTransferFundsBalance { get; set; }
        public decimal EstimatedProjectionBalance { get; set; }
        public decimal FundsIn { get; set; }
        public Cost AllModelledCosts { get; set; }
    }
}
