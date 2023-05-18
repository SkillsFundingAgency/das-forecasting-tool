namespace SFA.DAS.Forecasting.Models.Payments
{
    public class CalendarPeriod
    {
        public CalendarPeriod(int year, int month)
        {
            Year = year;
            Month = month;
        }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}