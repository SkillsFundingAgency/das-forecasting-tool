using System;
using System.Globalization;

namespace SFA.DAS.Forecasting.AcceptanceTests
{
    public static class Extensions
    {
        public static DateTime ToDateTime(this string value)
        {
            switch (value.ToLower())
            {
                case "today":
                    return DateTime.Today;
                case "now":
                    return DateTime.Now;
                case "yesterday":
                    return DateTime.Today.AddDays(-1);
                case "last week":
                    return DateTime.Today.AddDays(-7);
                case "last month":
                    return DateTime.Today.AddMonths(-1);
                case "last year":
                    return DateTime.Today.AddYears(-1);
                case "tomorrow":
                    return DateTime.Now.AddDays(1);
                case "next week":
                    return DateTime.Today.AddDays(7);
                case "next month":
                    return DateTime.Today.AddMonths(1);
                case "next year":
                    return DateTime.Today.AddYears(1);
                default:
                    return DateTime.ParseExact(value, new [] { "dd/MM/yyyy", "d/M/yyyy HH:mm", "dd/MM/yyyy HH:mm" }, new CultureInfo("en-GB"), DateTimeStyles.AllowWhiteSpaces);
            }
        }
    }
}