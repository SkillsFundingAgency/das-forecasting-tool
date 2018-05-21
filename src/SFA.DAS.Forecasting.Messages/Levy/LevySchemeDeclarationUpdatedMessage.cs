using System;

namespace SFA.DAS.Forecasting.Application.Levy.Messages
{
    /// <summary>
    /// TODO: Temp event definition. this will be replaced by the actual Levy event published by the employer services.
    /// </summary>
    public class LevySchemeDeclarationUpdatedMessage
    {
        public long AccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public long Id { get; set; }
        public string EmpRef { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string PayrollYear { get; set; }
        public short? PayrollMonth { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool EndOfYearAdjustment { get; set; }
        public decimal EndOfYearAdjustmentAmount { get; set; }
        public decimal LevyAllowanceForYear { get; set; }
        public DateTime? DateCeased { get; set; }
        public DateTime? InactiveFrom { get; set; }
        public DateTime? InactiveTo { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal LevyDeclaredInMonth { get; set; }
    }
}