using System;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class ProjectiontemViewModel
    {
        public DateTime Date { get; set; }

        public decimal FundsIn { get; set; }

        public decimal CostOfTraining { get; set; }

        public decimal CompletionPayments { get; set; }

        public decimal ExpiredFunds { get; set; }

        public decimal Balance { get; set; }

        public decimal CoInvestmentEmployer { get; set; }

        public decimal CoInvestmentGovernment { get; set; }
    }
}