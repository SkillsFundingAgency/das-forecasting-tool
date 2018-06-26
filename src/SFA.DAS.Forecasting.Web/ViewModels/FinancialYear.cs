using System;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class FinancialYear
    {
        public FinancialYear(DateTime date)
        {
            var year = date.Month <= 3 ? date.Year - 1 : date.Year;
            StartDate = new DateTime(year, 04, 01);

            EndDate = StartDate.AddYears(1).AddMonths(-1);

            FirstStartDate = StartDate;
            FirstStartDate = EndDate;
        }

        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public DateTime FirstStartDate { get; internal set; }
        public DateTime LastEndDate { get; internal set; }
    }
}