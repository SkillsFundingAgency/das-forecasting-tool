using System;

namespace SFA.DAS.Forecasting.AcceptanceTests.Levy
{
    public class LevySubmission
    {
        public string Scheme { get; set; }
        public decimal Amount { get; set; }
        public string CreatedDate { get; set; }

        public DateTime CreatedDateValue => CreatedDate.Equals("TODAY", StringComparison.OrdinalIgnoreCase)
            ? DateTime.Today
            : DateTime.Parse(CreatedDate);
    }
}