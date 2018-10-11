using CsvHelper.Configuration.Attributes;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class ApprenticeshipCsvItemViewModel
    {
        [Name("ILR actual start date")]
        public string StartDate { get; set; }
        [Name("ILR planned end date")]
        public string PlannedEndDate { get; set; }

        [Name("Apprenticeship service start date")]
        public string DasStartDate { get; set; }
        [Name("Apprenticeship service end date")]
        public string DasPlannedEndDate { get; set; }

        [Name("Apprenticeship")]
        public string Apprenticeship { get; set; }
        [Name("Apprenticeship Level")]
        public int? ApprenticeshipLevel { get; set; }
        [Name("Transfer to Employer")]
        public string TransferToEmployer { get; set; }
        [Name("ULN")]
        public string Uln { get; set; }
        [Name("Apprentice Name")]
        public string ApprenticeName { get; set; }
        [Name("UKPRN")]
        public long UkPrn { get; set; }
        [Name("Provider Name")]
        public string ProviderName { get; set; }
        [Name("Total Cost")]
        public int TotalCost { get; set; }
        [Name("Monthly Training Cost")]
        public int MonthlyTrainingCost { get; set; }
        [Name("Completion Amount")]
        public int CompletionAmount { get; set; }

        [Name("Apprenticeship Status")]
        public string Status { get; set; }
    }
}