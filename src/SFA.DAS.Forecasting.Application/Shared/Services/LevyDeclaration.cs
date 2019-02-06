using System;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public class LevyDeclaration
    {
        public string EmpRef { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string PayrollYear { get; set; }
        public short? PayrollMonth { get; set; }
        public decimal Amount { get; set; }
    }
}