using SFA.DAS.Forecasting.Models.Payments;
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
        public string ActualEndDate { get; set; }
        public DateTime? ActualEndDateValue => ActualEndDate.ToDateTime();
        public decimal InstallmentAmount { get; set; }
        public decimal CompletionAmount { get; set; }
        public int NumberOfInstallments { get; set; }
        public long SendingEmployerAccountId { get; set; }
        public FundingSource? FundingSource { get; set; }
        public long? EmployerAccountId { get; internal set; }
        public int ApprenticeshipId { get; internal set; }
    }
}