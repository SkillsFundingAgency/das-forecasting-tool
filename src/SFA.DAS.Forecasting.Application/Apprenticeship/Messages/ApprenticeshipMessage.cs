using System;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Application.Apprenticeship.Messages
{
    public class ApprenticeshipMessage
    {
        public long EmployerAccountId { get; set; }
        public long? SendingEmployerAccountId { get; set; }
        public long LearnerId { get; set; }
        public long ProviderId { get; set; }
        public string ProviderName { get; set; }
        public long ApprenticeshipId { get; set; }
        public string ApprenticeName { get; set; }
        public string CourseName { get; set; }
        public int CourseLevel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal CompletionAmount { get; set; }
        public decimal MonthlyInstallment { get; set; }
        public int NumberOfInstallments { get; set; }
        public FundingSource FundingSource { get; set; }
    }
}