using System;

namespace SFA.DAS.Forecasting.AcceptanceTests.Payments
{
    public class TestCommitment
    {
        public int LearnerId { get; set; }
        public string ApprenticeName { get; set; }
        public string CourseName { get; set; }
        public int CourseLevel { get; set; }
        public string ProviderName { get; set; }
        public string StartDate { get; set; }
        public DateTime StartDateValue => StartDate.ToDateTime();
        public DateTime PlannedEndDate => StartDateValue.AddMonths(NumberOfInstallments);
        public decimal InstallmentAmount { get; set; }
        public decimal CompletionAmount { get; set; }
        public int NumberOfInstallments { get; set; }
    }
}