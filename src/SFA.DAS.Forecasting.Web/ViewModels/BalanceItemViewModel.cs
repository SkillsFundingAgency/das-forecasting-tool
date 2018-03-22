using System;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class ProjectionsItemViewModel
    {
        public DateTime Date { get; set; }

        public decimal LevyCredit { get; set; }

        public decimal CostOfTraining { get; set; }

        public decimal CompletionPayments { get; set; }

        public decimal ExpiredFunds { get; set; }

        public decimal Balance { get; set; }

        public decimal CoInvestmentEmployer { get; set; }

        public decimal CoInvestmentGovernment { get; set; }
    }
}