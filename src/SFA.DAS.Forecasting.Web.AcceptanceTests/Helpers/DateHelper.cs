using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.Helpers
{
    static class DateHelper
    {
        public static int GetMonthNumber(string dateString)
        {
            if (dateString.StartsWith("Jan"))
                return 1;
            else if (dateString.StartsWith("Feb"))
                return 2;
            else if (dateString.StartsWith("Mar"))
                return 3;
            else if (dateString.StartsWith("Apr"))
                return 4;
            else if (dateString.StartsWith("May"))
                return 5;
            else if (dateString.StartsWith("Jun"))
                return 6;
            else if (dateString.StartsWith("Jul"))
                return 7;
            else if (dateString.StartsWith("Aug"))
                return 8;
            else if (dateString.StartsWith("Sept"))
                return 9;
            else if (dateString.StartsWith("Oct"))
                return 10;
            else if (dateString.StartsWith("Nov"))
                return 11;
            else
                return 12;
        }

        public static string GetMonthString(int month) {
            var values = new string[] {"Jan",
                "Feb",
                "Mar",
                "Apr",
                "May",
                "Jun",
                "Jul",
                "Aug",
                "Sept",
                "Oct",
                "Nov",
                "Dec"
                };
            if (month < 1 || month > 12)
            {
                throw new Exception("Invalid month format");
            }
            return values[month - 1];
        }

        public static string[] BuildDateRange(string from, string to)
        {
            var startMonth = GetMonthNumber(from.Split(' ').First());
            var endMonth = GetMonthNumber(from.Split(' ').First());
            var startYear = int.Parse(from.Split(' ').ElementAt(1));
            var endYear = int.Parse(from.Split(' ').ElementAt(1));
            if(startYear > endYear)
            {
                throw new Exception("Invalid from and to date values");
            }
            var currentMonth = startMonth;
            var results = new List<string>();
            for(var currentYear = startYear; ;)
            {
                var month = GetMonthString(currentMonth);
                var result = $"{month} {currentYear}";
                if(result.Equals(to))
                {
                    results.Add(result);
                    break;
                }
                results.Add(result);
                currentMonth++;
                if(currentMonth > 12)
                {
                    currentMonth = 1;
                    currentYear++;
                }
            }
            return results.ToArray();
        }
    }
}
