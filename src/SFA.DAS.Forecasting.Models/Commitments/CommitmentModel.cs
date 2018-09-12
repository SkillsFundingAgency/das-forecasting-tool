using System;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Models.Commitments
{
    public class CommitmentModel
    {
        public long Id { get; set; }
        public long EmployerAccountId { get; set; }
        public long SendingEmployerAccountId { get; set; }
        public long ApprenticeshipId { get; set; }
        public long LearnerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal CompletionAmount { get; set; }
        public decimal MonthlyInstallment { get; set; }
        public short NumberOfInstallments { get; set; }
        public long ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string ApprenticeName { get; set; }
        public string CourseName { get; set; }
        public int? CourseLevel { get; set; }
        public FundingSource FundingSource { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool HasHadPayment { get; set; }
    }
}