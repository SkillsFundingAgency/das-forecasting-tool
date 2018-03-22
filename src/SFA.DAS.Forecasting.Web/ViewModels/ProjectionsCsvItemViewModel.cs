using CsvHelper.Configuration.Attributes;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class ProjectionsCsvItemViewModel
    {
        public string Date { get; set; }

        [Name("Funds in")]
        public decimal LevyCredit { get; set; }

        [Name("Cost of training")]
        public decimal CostOfTraining { get; set; }

        [Name("Completion payments")]
        public decimal CompletionPayments { get; set; }

        [Name("Your contribution")]
        public decimal CoInvestmentEmployer { get; set; }

        [Name("Government contribution")]
        public decimal CoInvestmentGovernment { get; set; }

        [Name("Future funds")]
        public decimal Balance { get; set; }
    }
}