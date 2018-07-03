namespace SFA.DAS.Forecasting.Domain.Shared.Validation
{
    public class AddApprenticeshipValidationDetail
    {
        public string NoApprenticeshipSelected { get; set; }
        public string NoNumberOfApprentices { get; set; }
        public string NoNumberOfMonths { get; set; }
        public string ShortNumberOfMonths { get; set; }
        public string NoStartMonth { get; set; }
        public string InvalidStartMonth { get; set; }
        public string NoStartYear { get; set; }
        public string StartDateInPast { get; set; }

        public string LateDate { get; set; }
        public string OverCap { get; set; }
        public string NoCost { get; set; }

        public bool IsValid { get; set; }
        public string ValidationSummary { get; set; }
    }
}
