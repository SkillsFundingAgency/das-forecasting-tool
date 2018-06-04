using System;

namespace SFA.DAS.Forecasting.Domain.Shared
{
    public interface IDateTimeService
    {
        DateTime GetCurrentDateTime();
    }

    public class DateTimeService : IDateTimeService
    {
        public DateTime GetCurrentDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}