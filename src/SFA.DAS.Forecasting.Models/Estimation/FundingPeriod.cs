using System;

namespace SFA.DAS.Forecasting.Models.Estimation
{
    public class FundingPeriod
    {
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public int FundingCap { get; set; }
    }
}
