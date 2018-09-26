using System;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public class PeriodInformation
    {
        public string PeriodEndId { get; set; }
        public int CalendarPeriodMonth { get; set; }
        public int CalendarPeriodYear { get; set; }
    }
}