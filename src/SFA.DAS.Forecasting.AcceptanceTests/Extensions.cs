using System;
using System.Data.Entity.Infrastructure;
using System.Globalization;

namespace SFA.DAS.Forecasting.AcceptanceTests
{
    public static class Extensions
    {
        public static DateTime ToDateTime(this string value)
        {
            switch (value?.ToLower() ?? "minvalue")
            {
                case "today":
                    return DateTime.Today;
                case "now":
                    return DateTime.Now;
                case "yesterday":
                    return DateTime.Now.AddDays(-1);
                case "this month":
                    return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
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
                case "minvalue":
                    return DateTime.MinValue;
                case string s when s.Contains("months ago"):
                    var months = int.Parse("-" + value?.Split(' ')[0]);
                    return DateTime.Today.AddMonths(months);
                default:
                    if (DateTime.TryParseExact(value,
                        new[] {"dd/MM/yyyy", "d/M/yyyy HH:mm", "dd/MM/yyyy HH:mm"}, new CultureInfo("en-GB"),
                        DateTimeStyles.AllowWhiteSpaces, out var result))
                    {
                        return result;
                    }

                    return result;
                    
            }
        }
    }
}