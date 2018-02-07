using System;

namespace SFA.DAS.Forecasting.Domain.Commitments.Model
{
    public class Commitment
    {
        public long EmployerAccountId { get; set; }
        public long ApprenticeshipId { get; set; }
        public long LearnerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal CompletionAmount { get; set; }
        public decimal MonthlyInstallment { get; set; }
        public short NumberOfInstallments { get; set; }
    }
}