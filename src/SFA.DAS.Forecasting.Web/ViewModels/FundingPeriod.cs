using System;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class FundingPeriodViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal FundingCap { get; set; }
    }
}