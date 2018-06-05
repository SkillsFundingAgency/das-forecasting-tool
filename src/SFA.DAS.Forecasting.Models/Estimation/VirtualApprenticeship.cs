using SFA.DAS.Forecasting.Models.Payments;
using System;

namespace SFA.DAS.Forecasting.Models.Estimation
{
    public class VirtualApprenticeship
    {
        public string Id { get; set; }
        public string CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int Level { get; set; }
        public int ApprenticesCount { get; set; }
        public DateTime StartDate { get; set; }
        public short TotalInstallments { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalCompletionAmount { get; set; }
        public decimal TotalInstallmentAmount { get; set; }
        public FundingSource FundingSource { get; set; }
    }
}
